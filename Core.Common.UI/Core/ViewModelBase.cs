using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Core;
using System.ComponentModel;
using FluentValidation.Results;

namespace Core.Common.UI.Core
{
    public class ViewModelBase : ObjectBase
    {
        public ViewModelBase()
        {
            ToggleErrorsCommand = new DelegateCommand<object>(OnToggleErrorsCommandExecute, OnToggleErrorsCommandCanExecute);
        }

        bool _ErrorsVisible = false;

        public object ViewLoaded
        {
            get
            {
                OnViewLoaded();
                return null;
            }
        }

        protected virtual void OnViewLoaded() { }

        protected void WithClient<T>(T proxy, Action<T> codeToExecute)
        {
            codeToExecute.Invoke(proxy);

            IDisposable disposableClient = proxy as IDisposable;
            if (disposableClient != null)
                disposableClient.Dispose();
        }

        public virtual string ViewTitle
        {
            get { return String.Empty; }
        }

        List<ObjectBase> _Models;

        protected virtual void AddModels(List<ObjectBase> models) { }

        protected void ValidateModel()
        {
            if (_Models == null)
            {
                _Models = new List<ObjectBase>();
                AddModels(_Models);
            }

            _ValidationErrors = new List<ValidationFailure>();

            if (_Models.Count > 0)
            {
                foreach (ObjectBase modelObject in _Models)
                {
                    if (modelObject != null)
                        modelObject.Validate();

                    _ValidationErrors = _ValidationErrors.Union(modelObject.ValidationErrors).ToList();
                }

                OnPropertyChanged(() => ValidationErrors, false);
                OnPropertyChanged(() => ValidationHeaderText, false);
                OnPropertyChanged(() => ValidationHeaderVisible, false);
            }
        }

        public DelegateCommand<object> ToggleErrorsCommand { get; protected set; }

        public virtual bool ValidationHeaderVisible
        {
            get { return ValidationErrors != null && ValidationErrors.Count() > 0; }
        }

        public virtual bool ErrorsVisible
        {
            get { return _ErrorsVisible; }
            set
            {
                if (_ErrorsVisible == value)
                    return;

                _ErrorsVisible = value;
                OnPropertyChanged(() => ErrorsVisible, false);
            }
        }

        public virtual string ValidationHeaderText
        {
            get
            {
                string ret = string.Empty;

                if (ValidationErrors != null)
                {
                    string verb = (ValidationErrors.Count() == 1 ? "is" : "are");
                    string suffix = (ValidationErrors.Count() == 1 ? "" : "s");

                    if (!IsValid)
                        ret = string.Format("There {0} {1} validation error{2}.", verb, ValidationErrors.Count(), suffix);
                }

                return ret;
            }
        }

        protected virtual void OnToggleErrorsCommandExecute(object arg)
        {
            ErrorsVisible = !ErrorsVisible;
        }

        protected virtual bool OnToggleErrorsCommandCanExecute(object arg)
        {
            return !IsValid;
        }
    }
}
