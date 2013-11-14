using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Core.Common.Contracts;

namespace Core.Common.Core
{
    public class CollectionBase<T> : ObservableCollection<T>, IDirtyCapable
           where T : ObjectBase
    {
        #region Property change notification
        
        event ItemPropertyChangedEventHandler<T> _ItemPropertyChangedEvent;

        protected List<ItemPropertyChangedEventHandler<T>> _ItemPropertyChangedSubscribers = new List<ItemPropertyChangedEventHandler<T>>();

        public event ItemPropertyChangedEventHandler<T> ItemPropertyChanged
        {
            add
            {
                if (!_ItemPropertyChangedSubscribers.Contains(value))
                {
                    _ItemPropertyChangedEvent += value;
                    _ItemPropertyChangedSubscribers.Add(value);
                }
            }
            remove
            {
                _ItemPropertyChangedEvent -= value;
                _ItemPropertyChangedSubscribers.Remove(value);
            }
        }


        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnItemPropertyChanged((T)sender, e.PropertyName);
        }

        protected virtual void OnItemPropertyChanged(T item, string propertyName)
        {
            if (_ItemPropertyChangedEvent != null)
                _ItemPropertyChangedEvent(this, new ItemPropertyChangedEventArgs<T>(item, propertyName));
        }

        #endregion

        #region IDirtyCapable members

        [NotNavigable]
        public virtual bool IsDirty
        {
            get
            {
                bool isDirty = false;
                
                foreach (var item in this)
                {
                    if (item.IsDirty)
                    {
                        isDirty = true;
                        break;
                    }
                }

                return isDirty;
            }
        }

        public virtual bool IsAnythingDirty()
        {
            bool isDirty = false;

            foreach (var item in this)
            {
                if (item.IsAnythingDirty())
                {
                    isDirty = true;
                    break;
                }
            }

            return isDirty;
        }

        public List<IDirtyCapable> GetDirtyObjects()
        {
            List<IDirtyCapable> dirtyObjects = new List<IDirtyCapable>();

            foreach (var item in this)
                dirtyObjects.AddRange(item.GetDirtyObjects());

            return dirtyObjects;
        }

        public void CleanAll()
        {
            foreach (var item in this)
                item.CleanAll();
        }

        #endregion

        #region ObservableCollection<T> overrides

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            item.PropertyChanged += item_PropertyChanged;
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);
            item.PropertyChanged -= item_PropertyChanged;
        }

        protected override void ClearItems()
        {
            foreach (var item in this)
                item.PropertyChanged -= item_PropertyChanged;

            base.ClearItems();
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            oldItem.PropertyChanged -= item_PropertyChanged;
            base.SetItem(index, item);
            item.PropertyChanged += item_PropertyChanged;
        }

        #endregion
    }
}
