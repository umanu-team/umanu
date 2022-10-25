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

namespace Framework.Presentation.Forms {

    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Base class of panes for combining fields, view panes or
    /// collections of objects visually.
    /// </summary>
    public class ViewPane : ViewObject {

        /// <summary>
        /// Internal key of this field.
        /// </summary>
        public string Key {
            get { return this.key.Value; }
            set { this.key.Value = value; }
        }
        private readonly PersistentFieldForString key =
            new PersistentFieldForString(nameof(Key));

        /// <summary>
        /// Internal key chain of this field.
        /// </summary>
        public string[] KeyChain {
            get { return Model.KeyChain.FromKey(this.Key); }
            set { this.Key = Model.KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewPane()
            : base() {
            this.RegisterPersistentField(this.key);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">internal key of field containing child
        /// object(s) to view</param>
        public ViewPane(string key)
            : this() {
            this.Key = key;
        }

        /// <summary>
        /// Finds the first view field for a specific key.
        /// </summary>
        /// <param name="key">key to find first view field for</param>
        /// <returns>first view field for specific key or null</returns>
        public ViewFieldForEditableValue FindOneViewField(string key) {
            return this.FindOneViewField(Model.KeyChain.FromKey(key));
        }

        /// <summary>
        /// Finds the first view field for a specific key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find first view field
        /// for</param>
        /// <returns>first view field for specific key chain or null</returns>
        public ViewFieldForEditableValue FindOneViewField(string[] keyChain) {
            ViewFieldForEditableValue viewField;
            var viewFields = this.FindViewFields(keyChain);
            if (viewFields.Count > 0) {
                viewField = viewFields[0];
            } else {
                viewField = null;
            }
            return viewField;
        }

        /// <summary>
        /// Finds the view fields for a specific key.
        /// </summary>
        /// <param name="key">key to find view fields for</param>
        /// <returns>view fields for specific key</returns>
        public ReadOnlyCollection<ViewFieldForEditableValue> FindViewFields(string key) {
            return this.FindViewFields(Model.KeyChain.FromKey(key));
        }

        /// <summary>
        /// Finds the view fields for a specific key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find view fields for</param>
        /// <returns>view fields for specific key chain</returns>
        public virtual ReadOnlyCollection<ViewFieldForEditableValue> FindViewFields(string[] keyChain) {
            throw new NotImplementedException(nameof(FindViewFields) + "() is not implemented in derived class of " + nameof(ViewPane) + ".");
        }

        /// <summary>
        /// Finds the view fields for a specific key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find view fields for</param>
        /// <param name="viewField">view field to search in</param>
        /// <returns>view fields for specific key chain</returns>
        protected static ReadOnlyCollection<ViewFieldForEditableValue> FindViewFields(string[] keyChain, ViewField viewField) {
            return ViewPane.FindViewFields(keyChain, new ViewField[] { viewField });
        }

        /// <summary>
        /// Finds the view fields for a specific key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find view fields for</param>
        /// <param name="viewFields">view fields to search in</param>
        /// <returns>view fields for specific key chain</returns>
        protected static ReadOnlyCollection<ViewFieldForEditableValue> FindViewFields(string[] keyChain, IEnumerable<ViewField> viewFields) {
            var matchingViewFields = new List<ViewFieldForEditableValue>();
            if (keyChain.LongLength > 0L) {
                string key = Model.KeyChain.ToKey(keyChain);
                foreach (var viewField in viewFields) {
                    var viewFieldForEditableValue = viewField as ViewFieldForEditableValue;
                    if (null != viewFieldForEditableValue && viewFieldForEditableValue.Key == key) {
                        matchingViewFields.Add(viewFieldForEditableValue);
                    }
                }
            }
            return matchingViewFields.AsReadOnly();
        }

        /// <summary>
        /// Finds the view fields for a specific key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find view fields for</param>
        /// <param name="viewPanes">view panes to search in</param>
        /// <returns>view fields for specific key chain</returns>
        protected internal static ReadOnlyCollection<ViewFieldForEditableValue> FindViewFields(string[] keyChain, IEnumerable<ViewPane> viewPanes) {
            var matchingViewFields = new List<ViewFieldForEditableValue>();
            if (keyChain.LongLength > 0L) {
                string key = keyChain[0];
                foreach (var viewPane in viewPanes) {
                    if (viewPane.KeyChain.LongLength < 1) {
                        matchingViewFields.AddRange(viewPane.FindViewFields(keyChain));
                    } else if (Model.KeyChain.StartsWith(keyChain, viewPane.KeyChain)) {
                        var remainingKeyChain = Model.KeyChain.RemoveLeadingLinksOf(keyChain, viewPane.KeyChain.LongLength);
                        matchingViewFields.AddRange(viewPane.FindViewFields(remainingKeyChain));
                    }
                }
            }
            return matchingViewFields.AsReadOnly();
        }

        /// <summary>
        /// Applies the key chain to a presentable object to find the
        /// matching child object for this view.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// apply key chain to</param>
        /// <returns>presentable object for this view</returns>
        internal IPresentableObject FindPresentableChildObject(IPresentableObject presentableObject) {
            IPresentableObject childObject;
            if (this.KeyChain.LongLength > 0) {
                var presentableField = presentableObject.FindPresentableField(this.KeyChain) as IPresentableFieldForElement;
                if (null == presentableField) {
                    childObject = null;
                } else {
                    childObject = presentableField.ValueAsObject as IPresentableObject;
                }
            } else {
                childObject = presentableObject;
            }
            return childObject;
        }

        /// <summary>
        /// Applies the key chain to a presentable object to find the
        /// matching child objects for this view.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// apply key chain to</param>
        /// <param name="createPotentialNewObjects">true to include
        /// objects which could be created potentionally, false
        /// otherwise</param>
        /// <returns>presentable objects for this view</returns>
        internal IEnumerable<IPresentableObject> FindPresentableChildObjects(IPresentableObject presentableObject, bool createPotentialNewObjects) {
            if (this.KeyChain.LongLength > 0) {
                var presentableField = presentableObject.FindPresentableField(this.KeyChain) as IPresentableFieldForCollection;
                if (null != presentableField) {
                    if (createPotentialNewObjects && !presentableField.IsReadOnly) {
                        yield return presentableField.NewItemAsObject() as IPresentableObject;
                    }
                    foreach (var currentObject in presentableField.GetValuesAsObject()) {
                        yield return currentObject as IPresentableObject;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all view fields for a presentable object and all
        /// child objects as a flattened enumerable. All key chains
        /// are converted to be relative to topmost presenable
        /// object.
        /// </summary>
        /// <param name="keyChainPath">key chain path to topmost
        /// presentable object</param>
        /// <param name="isParentVisible">true if parent is visible,
        /// false otherwise</param>
        /// <returns>flattened view fields for presentable object
        /// with converted key chains</returns>
        internal virtual IEnumerable<ViewField> GetViewFieldsCascadedly(string[] keyChainPath, bool isParentVisible) {
            throw new NotImplementedException(nameof(GetViewFieldsCascadedly) + "() is not implemented in derived class of " + nameof(ViewPane) + ".");
        }

        /// <summary>
        /// Gets all view fields for a presentable object and all
        /// child objects as a flattened enumerable. All key chains
        /// are converted to be relative to topmost presenable
        /// object.
        /// </summary>
        /// <param name="keyChainPath">key chain path to topmost
        /// presentable object</param>
        /// <param name="isParentVisible">true if parent is visible,
        /// false otherwise</param>
        /// <param name="viewFields">view fields to get converted
        /// copies for</param>
        /// <returns>flattened view fields for presentable object
        /// with converted key chains</returns>
        protected IEnumerable<ViewField> GetViewFieldsCascadedly(string[] keyChainPath, bool isParentVisible, IEnumerable<ViewField> viewFields) {
            string[] childKeyChainPath;
            if (string.IsNullOrEmpty(this.Key)) {
                childKeyChainPath = keyChainPath;
            } else {
                childKeyChainPath = Model.KeyChain.Concat(keyChainPath, this.KeyChain);
            }
            foreach (var viewField in viewFields) {
                var copiedViewField = Activator.CreateInstance(viewField.Type) as ViewField;
                copiedViewField.CopyFrom(viewField, CopyBehaviorForAllowedGroups.DoNotCopy, CopyBehaviorForAggregations.ShallowCopy, CopyBehaviorForCompositions.DeepCopy);
                copiedViewField.IsVisible = copiedViewField.IsVisible && isParentVisible;
                if (copiedViewField is ViewFieldForEditableValue viewFieldForEditableValue) {
                    viewFieldForEditableValue.KeyChain = Model.KeyChain.Concat(childKeyChainPath, viewFieldForEditableValue.KeyChain);
                }
                if (viewField is IClickableViewFieldWithLookupProvider clickableViewFieldWithLookupProvider && null != clickableViewFieldWithLookupProvider.OnClickUrlDelegate) {
                    ((IClickableViewFieldWithLookupProvider)copiedViewField).OnClickUrlDelegate = clickableViewFieldWithLookupProvider.OnClickUrlDelegate;
                }
                if (viewField is IClickableViewFieldWithOptionProvider clickableViewFieldWithOptionProvider && null != clickableViewFieldWithOptionProvider.OnClickUrlDelegate) {
                    ((IClickableViewFieldWithOptionProvider)copiedViewField).OnClickUrlDelegate = clickableViewFieldWithOptionProvider.OnClickUrlDelegate;
                }
                yield return copiedViewField;
            }
        }

        /// <summary>
        /// Gets all view fields for a presentable object and all
        /// child objects as a flattened enumerable. All key chains
        /// are converted to be relative to topmost presenable
        /// object.
        /// </summary>
        /// <param name="keyChainPath">key chain path to topmost
        /// presentable object</param>
        /// <param name="isParentVisible">true if parent is visible,
        /// false otherwise</param>
        /// <param name="viewPanes">view panes to get view fields for</param>
        /// <returns>flattened view fields for presentable object
        /// with converted key chains</returns>
        protected IEnumerable<ViewField> GetViewFieldsCascadedly(string[] keyChainPath, bool isParentVisible, IEnumerable<ViewPane> viewPanes) {
            string[] childKeyChainPath;
            if (string.IsNullOrEmpty(this.Key)) {
                childKeyChainPath = keyChainPath;
            } else {
                childKeyChainPath = Model.KeyChain.Concat(keyChainPath, this.KeyChain);
            }
            bool isVisible = this.IsVisible && isParentVisible;
            foreach (var viewPane in viewPanes) {
                foreach (var viewField in viewPane.GetViewFieldsCascadedly(childKeyChainPath, isVisible)) {
                    yield return viewField;
                }
            }
        }

        /// <summary>
        /// Determines whether this view pane is read-only for a
        /// specific presentable object.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// check</param>
        /// <returns>true if this form view is read-only for the
        /// given presentable object, false otherwise</returns>
        internal virtual bool IsReadOnlyFor(IPresentableObject presentableObject) {
            throw new NotImplementedException(nameof(IsReadOnlyFor) + "() is not implemented in derived class of " + nameof(ViewPane) + ".");
        }

        /// <summary>
        /// Determines whether this view pane is read-only for a
        /// specific presentable object.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// check</param>
        /// <param name="viewFields">view fields to check</param>
        /// <returns>true if this form view is read-only for the
        /// given presentable object, false otherwise</returns>
        protected static bool IsReadOnlyFor(IPresentableObject presentableObject, IEnumerable<ViewField> viewFields) {
            bool isReadOnly = true;
            if (null != presentableObject) {
                foreach (var viewField in viewFields) {
                    var viewFieldForEditableValue = viewField as ViewFieldForEditableValue;
                    if (null != viewFieldForEditableValue && !viewFieldForEditableValue.IsReadOnly) {
                        IPresentableField presentableField = presentableObject.FindPresentableField(viewFieldForEditableValue.KeyChain);
                        if (null != presentableField && !presentableField.IsReadOnly) {
                            isReadOnly = false;
                            break;
                        }
                    }
                }
            }
            return isReadOnly;
        }

        /// <summary>
        /// Determines whether this view pane is read-only for a
        /// specific presentable object.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// check</param>
        /// <param name="viewPanes">view panes to check</param>
        /// <returns>true if this form view is read-only for the
        /// given presentable object, false otherwise</returns>
        protected static bool IsReadOnlyFor(IPresentableObject presentableObject, IEnumerable<ViewPane> viewPanes) {
            bool isReadOnly = true;
            if (null != presentableObject) {
                foreach (var viewPane in viewPanes) {
                    if (null != viewPane && !viewPane.IsReadOnlyFor(presentableObject)) {
                        isReadOnly = false;
                        break;
                    }
                }
            }
            return isReadOnly;
        }

        /// <summary>
        /// Returns true if the specified presentable object is
        /// valid, false otherwise.
        /// </summary>
        /// <param name="presentableObject">presentable object to be
        /// validated</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <param name="topmostPresentableObject">topmost presentable
        /// parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>true if the specified key is valid, false
        /// otherwise</returns>
        public virtual bool IsValidValue(IPresentableObject presentableObject, ValidityCheck validityCheck, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            throw new NotImplementedException(nameof(IsValidValue) + "() is not implemented in derived class of " + nameof(ViewPane) + ".");
        }

        /// <summary>
        /// Returns true if the specified presentable object is
        /// valid, false otherwise.
        /// </summary>
        /// <param name="viewObject">presentable object to be
        /// validated</param>
        /// <param name="viewFields">view fields to validate against</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <param name="topmostPresentableObject">topmost presentable
        /// parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>true if the presentable object is valid, false
        /// otherwise</returns>
        protected static bool IsValidValue(IPresentableObject viewObject, IEnumerable<ViewField> viewFields, ValidityCheck validityCheck, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            bool isValidValue = true;
            if (null != viewObject) {
                foreach (var viewField in viewFields) {
                    var viewFieldForEditableValue = viewField as ViewFieldForEditableValue;
                    if (null != viewFieldForEditableValue) {
                        IPresentableField presentableField = viewObject.FindPresentableField(viewFieldForEditableValue.KeyChain);
                        if (null != presentableField) {
                            if (presentableField.IsForSingleElement) {
                                var viewFieldForElement = viewFieldForEditableValue as ViewFieldForElement;
                                if (null != viewFieldForElement && !viewFieldForElement.IsReadOnly) {
                                    var presentableFieldForElement = presentableField as IPresentableFieldForElement;
                                    if (!presentableFieldForElement.IsReadOnly) {
                                        isValidValue = string.IsNullOrEmpty(viewFieldForElement.Validate(presentableFieldForElement, validityCheck, topmostPresentableObject, optionDataProvider));
                                    }
                                }
                            } else {
                                var viewFieldForCollection = viewFieldForEditableValue as ViewFieldForCollection;
                                if (null != viewFieldForCollection && !viewFieldForCollection.IsReadOnly) {
                                    var presentableFieldForCollection = presentableField as IPresentableFieldForCollection;
                                    if (!presentableFieldForCollection.IsReadOnly) {
                                        isValidValue = string.IsNullOrEmpty(viewFieldForCollection.Validate(presentableFieldForCollection, validityCheck, topmostPresentableObject, optionDataProvider));
                                    }
                                }
                            }
                            if (!isValidValue) {
                                break;
                            }
                        }
                    }
                }
            }
            return isValidValue;
        }

        /// <summary>
        /// Returns true if the specified presentable object is
        /// valid, false otherwise.
        /// </summary>
        /// <param name="viewObject">presentable object to be
        /// validated</param>
        /// <param name="viewPanes">view panes to validate against</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <param name="topmostPresentableObject">topmost presentable
        /// parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>true if the presentable object is valid, false
        /// otherwise</returns>
        protected static bool IsValidValue(IPresentableObject viewObject, IEnumerable<ViewPane> viewPanes, ValidityCheck validityCheck, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            bool isValidValue = true;
            if (null != viewObject) {
                foreach (var viewPane in viewPanes) {
                    if (null != viewPane && !viewPane.IsValidValue(viewObject, validityCheck, topmostPresentableObject, optionDataProvider)) {
                        isValidValue = false;
                        break;
                    }
                }
            }
            return isValidValue;
        }

        /// <summary>
        /// Sets all desired and required fields of view pane to the
        /// specified manditoriness.
        /// </summary>
        /// <param name="manditoriness">manditoriness to set for all
        /// desired and required fields of view pane</param>
        public virtual void SetMandatoriness(Mandatoriness manditoriness) {
            throw new NotImplementedException(nameof(SetMandatoriness) + "() is not implemented in derived class of " + nameof(ViewPane) + ".");
        }

        /// <summary>
        /// Sets all desired and required fields of view pane to the
        /// specified manditoriness.
        /// </summary>
        /// <param name="manditoriness">manditoriness to set for all
        /// desired and required fields of view pane</param>
        /// <param name="viewFields">view fields to set mandatoriness
        /// for</param>
        protected static void SetMandatoriness(Mandatoriness manditoriness, IEnumerable<ViewField> viewFields) {
            foreach (var viewField in viewFields) {
                var viewFieldForEditableValue = viewField as ViewFieldForEditableValue;
                if (null != viewFieldForEditableValue) {
                    if (Mandatoriness.Optional != viewFieldForEditableValue.Mandatoriness) {
                        viewFieldForEditableValue.Mandatoriness = manditoriness;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Sets all desired and required fields of view pane to the
        /// specified manditoriness.
        /// </summary>
        /// <param name="manditoriness">manditoriness to set for all
        /// desired and required fields of view pane</param>
        /// <param name="viewPanes">view panes to set mandatoriness
        /// for</param>
        protected static void SetMandatoriness(Mandatoriness manditoriness, IEnumerable<ViewPane> viewPanes) {
            foreach (var viewPane in viewPanes) {
                viewPane.SetMandatoriness(manditoriness);
            }
            return;
        }

        /// <summary>
        /// Sets all fields of view pane to be read-only.
        /// </summary>
        public virtual void SetReadOnly() {
            throw new NotImplementedException(nameof(SetReadOnly) + "() is not implemented in derived class of " + nameof(ViewPane) + ".");
        }

        /// <summary>
        /// Sets all fields of view pane to be read-only.
        /// </summary>
        /// <param name="viewFields">view fields to set to be
        /// read-only</param>
        protected static void SetReadOnly(IEnumerable<ViewField> viewFields) {
            foreach (var viewField in viewFields) {
                if (viewField is ViewFieldForEditableValue viewFieldForEditableValue) {
                    viewFieldForEditableValue.IsReadOnly = true;
                }
            }
            return;
        }

        /// <summary>
        /// Sets all fields of view pane to be read-only.
        /// </summary>
        /// <param name="viewPanes">view panes to set to be
        /// read-only</param>
        protected static void SetReadOnly(IEnumerable<ViewPane> viewPanes) {
            foreach (var viewPane in viewPanes) {
                viewPane.SetReadOnly();
            }
            return;
        }

    }

}