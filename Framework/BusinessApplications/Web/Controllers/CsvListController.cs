/*********************************************************************
 * Umanu Framework / (C) Umanu Team / http://www.umanu.de/           *
 *                                                                   *
 * This program is free software: you can redistribute it and/or     *
 * modify it under the terms of the GNU Lesser General Public        *
 * License as published by the Free Software Foundation, either      *
 * version 3 of the License, or (at your option) any later version.  *
 *                                                                   *
 * This program is distributed in the hope that it will be useful,   *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of    *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the     *
 * GNU Lesser General Public License for more details.               *
 *                                                                   *
 * You should have received a copy of the GNU Lesser General Public  *
 * License along with this program.                                  *
 * If not, see <http://www.gnu.org/licenses/>.                       *
 *********************************************************************/

namespace Framework.BusinessApplications.Web.Controllers {

    using Framework.BusinessApplications.DataControllers;
    using Framework.Model;
    using Framework.Persistence;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using Framework.Properties;
    using Presentation.Converters;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// HTTP controller for responding dynamic CSV files based on
    /// list table views.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public class CsvListController<T> : FileController where T : class, IProvidableObject {

        /// <summary>
        /// Absolute URL of dynamic CSV file - it may not be empty,
        /// not contain any special charaters except for dashes and
        /// has to start with a slash.
        /// </summary>
        public string AbsoluteUrl {
            get {
                return this.absoluteUrl;
            }
            private set {
                if (!value.StartsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of dynamic CSV file does not start with a slash.");
                }
                this.absoluteUrl = value;
            }
        }
        private string absoluteUrl;

        /// <summary>
        /// Business application to process.
        /// </summary>
        public IBusinessApplication BusinessApplication { get; private set; }

        /// <summary>
        /// Data controller for dynamic CSV file.
        /// </summary>
        public ListTableDataController<T> ListTableDataController { get; private set; }

        /// <summary>
        /// CSV separator to be used.
        /// </summary>
        public char Separator { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of dynamic CSV
        /// file - it may not be empty, not contain any special
        /// charaters except for dashes and has to start with a slash</param>
        /// <param name="listTableDataController">data controller for
        /// dynamic CSV file</param>
        public CsvListController(IBusinessApplication businessApplication, string absoluteUrl, ListTableDataController<T> listTableDataController)
            : base(CacheControl.NoStore, false) {
            this.AbsoluteUrl = absoluteUrl;
            this.BusinessApplication = businessApplication;
            this.ListTableDataController = listTableDataController;
            this.Separator = ';';
        }

        /// <summary>
        /// Finds the file for a specific URL.
        /// </summary>
        /// <param name="url">URL of requested file</param>
        /// <returns>file for URL or null</returns>
        protected sealed override File FindFile(Uri url) {
            File csvFile;
            if (url.AbsolutePath == this.AbsoluteUrl) {
                var startTime = UtcDateTime.Now;
                File jobQueueCsvFile = null;
                PlainTextEmail email = null;
                {
                    var fileName = url.Segments[url.Segments.LongLength - 1];
                    var optionDataProvider = new OptionDataProvider(this.BusinessApplication.PersistenceMechanism);
                    var providableObjects = this.GetProvidableObjects(url, optionDataProvider);
                    var dataProvider = this.ListTableDataController.DataProvider;
                    var keyChainsToPreload = this.ListTableDataController.GetKeyChainsToPreloadForListTableView();
                    var listView = this.ListTableDataController.GetListTableView();
                    JobQueue.Enqueue(delegate () {
                        jobQueueCsvFile = CsvListController<T>.GetCsvFile(fileName, providableObjects, dataProvider, keyChainsToPreload, listView, optionDataProvider); // usage of "this" is avoided intentionally
                        if (null != email) {
                            email.Attachments.Add(jobQueueCsvFile);
                            email.SendAsync();
                        }
                        return;
                    });
                }
                var task = Task.Run(delegate () { // task is not disposed intentionally: https://devblogs.microsoft.com/pfxteam/do-i-need-to-dispose-of-tasks/
                    while (null == jobQueueCsvFile) {
                        Thread.Sleep(100);
                    }
                });
                var waitTime = TimeSpan.FromMinutes(1) - (UtcDateTime.Now - startTime);
                if (waitTime < TimeSpan.Zero) {
                    waitTime = TimeSpan.FromSeconds(1);
                }
                if (task.Wait(waitTime)) { // job queue finished in time
                    csvFile = jobQueueCsvFile;
                } else { // job queue needs longer than expected
                    email = new PlainTextEmail {
                        Subject = Resources.Export + ": " + this.BusinessApplication.Title,
                        BodyText = Resources.PleaseFindAttachedTheRequestedDataExport
                    };
                    var to = this.BusinessApplication.PersistenceMechanism.UserDirectory.CurrentUser;
                    email.To.Add(to);
                    csvFile = this.GetMessageFile(string.Format(Resources.PleaseExcuseThatTheDataExportTakesLongerThanExpected, to.EmailAddress));
                }
            } else {
                csvFile = null;
            }
            return csvFile;
        }

        /// <summary>
        /// Gets the specific CSV file.
        /// </summary>
        /// <param name="fileName">name of CSV file</param>
        /// <param name="providableObjects">providable objects to be
        /// provided as CSV file</param>
        /// <param name="dataProvider">controller for CSV
        /// data</param>
        /// <param name="keyChainsToPreload">key chains to be
        /// preloaded</param>
        /// <param name="listView">list view to be applied for field
        /// mapping</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>specific CSV file</returns>
        private static File GetCsvFile(string fileName, IEnumerable<T> providableObjects, DataProvider<T> dataProvider, IEnumerable<string[]> keyChainsToPreload, IListTableView listView, OptionDataProvider optionDataProvider) {
            dataProvider.Preload(providableObjects, keyChainsToPreload);
            var csvWriter = new CsvWriter(listView, optionDataProvider);
            return csvWriter.WriteFile(fileName, providableObjects);
        }

        /// <summary>
        /// Gets an HTML file that displays a message to the user.
        /// </summary>
        /// <param name="message">message to be displayed, will not
        /// be escaped/encoded automatically</param>
        /// <returns>HTML file that displays a message to the user</returns>
        private File GetMessageFile(string message) {
            var html = new StringBuilder();
            html.Append("<!doctype html><html><head><meta charset=\"utf-8\" /><script type=\"text/javascript\">window.alert('");
            html.Append(message);
            html.Append("');window.history.back();</script></head></html>");
            return new File(string.Empty, "text/html", Encoding.UTF8.GetBytes(html.ToString()));
        }

        /// <summary>
        /// Gets the providable objects to be provided as CSV file
        /// </summary>
        /// <param name="url">URL of requested CSV file</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>providable objects to be provided as CSV file</returns>
        protected virtual ICollection<T> GetProvidableObjects(Uri url, OptionDataProvider optionDataProvider) {
            return new SubsetQueryResolver<T>(url, this.ListTableDataController.DataProvider, ulong.MaxValue).FindProvidableObjects();
        }

        /// <summary>
        /// Updates the parent persistent object of a file for a
        /// specific URL.
        /// </summary>
        /// <param name="url">URL of file to update parent persistent
        /// object for</param>
        /// <returns>true on success, false otherwise</returns>
        protected override bool UpdateParentPersistentObjectOfFile(Uri url) {
            return false;
        }

    }

}