using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Core.Common.Core
{
    public delegate void ItemPropertyChangedEventHandler<T>(object sender, ItemPropertyChangedEventArgs<T> e)
            where T : INotifyPropertyChanged;
}
