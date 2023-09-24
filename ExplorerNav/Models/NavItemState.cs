using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExplorerNav.Models
{
    [Flags]
    internal enum TypeEnum
    {
        //None = 0,
        Normal = 0,
        BuiltIn = 1
    }

    [Flags]
    internal enum AppliedEnum
    {
        None = 0,
        Applied = 2,
        Unapplied = 4
    }

    [Flags]
    internal enum SavedEnum
    {
        None = 0,
        Saved = 8,
        Unsaved = 16
    }

    [Flags]
    internal enum ErrorEnum
    {
        None = 0,
        ReadError = 32,
        WriteError = 64,
    }

    [Flags]
    internal enum AllStatesEnum
    {
        None = 0,
        BuiltIn = TypeEnum.BuiltIn,
        Applied = AppliedEnum.Applied,
        Unapplied = AppliedEnum.Unapplied,
        Saved = SavedEnum.Saved,
        Unsaved = SavedEnum.Unsaved,
        SavedBuiltIn = TypeEnum.BuiltIn + SavedEnum.Saved,
        UnsavedBuiltIn = TypeEnum.BuiltIn + SavedEnum.Unsaved,
        UnappliedUnsaved = AppliedEnum.Unapplied + SavedEnum.Unsaved,
        AppliedBuiltIn = TypeEnum.BuiltIn + AppliedEnum.Applied,
        UnappliedBuiltIn = TypeEnum.BuiltIn + AppliedEnum.Unapplied,
        ReadError = ErrorEnum.ReadError,
        WriteError = ErrorEnum.WriteError,
    }

    internal class NavItemState : INotifyPropertyChanged
    {
        internal record struct StatusData(
            Dictionary<string, int> data
        );

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public StateTracker<NavItemKeys> Fields { get; private set; }

        private TypeEnum _type = 0;
        private AppliedEnum _appliedStatus = AppliedEnum.None;
        private SavedEnum _savedStatus = SavedEnum.None;
        private ErrorEnum _errorStatus = ErrorEnum.None;
        private string _errorMessage = "";

        public TypeEnum Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
                UpdateType();
            }
        }

        public ErrorEnum ErrorStatus
        {
            get => _errorStatus;
            set
            {
                _errorStatus = value;
                OnPropertyChanged(nameof(ErrorStatus));
                OnPropertyChanged(nameof(Status));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public StatusData Status
        {
            get
            {
                var data = new Dictionary<string, int>
                {
                    { "applied", ((int)AppliedStatus + (int)Type) },
                    { "saved", (int)SavedStatus},
                    { "error", (int)ErrorStatus },
                };

                return new()
                {
                    data = data,
                };
            }
        }

        public AppliedEnum AppliedStatus
        {
            get => _appliedStatus;
            set
            {
                _appliedStatus = value;
                UpdateApplyStatus();
            }
        }

        public SavedEnum SavedStatus
        {
            get => _savedStatus;
            set
            {
                if (value == SavedEnum.Unsaved && !IsApplied) return;
                _savedStatus = value;
                UpdateSavedStatus();
            }
        }

        public bool IsNormal
        {
            get => Type == TypeEnum.Normal;
        }

        public bool IsBuiltIn
        {
            get => Type == TypeEnum.BuiltIn;
        }

        public bool IsApplied
        {
            get => AppliedStatus == AppliedEnum.Applied;
        }

        public bool IsSaved
        {
            get => SavedStatus == SavedEnum.Saved;
        }

        public bool IsUnsaved
        {
            get => SavedStatus == SavedEnum.Unsaved;
        }

        public NavItemState(NavItemKeys[] _keys)
        {
            Fields = new StateTracker<NavItemKeys>(_keys);
        }

        public void SetType(bool isBuiltIn)
        {
            Type = isBuiltIn ? TypeEnum.BuiltIn : TypeEnum.Normal;
        }

        public void SetApplied(bool isApplied)
        {
            AppliedStatus = isApplied ? AppliedEnum.Applied : AppliedEnum.Unapplied;
            UpdateApplyStatus();
        }

        public void SetSaved(bool isSaved)
        {
            SavedStatus = isSaved ? SavedEnum.Saved : SavedEnum.Unsaved;
            UpdateSavedStatus();
        }

        public void RefreshStatus()
        {
            UpdateSavedStatus();
            UpdateApplyStatus();
        }

        public void UpdateSaveStatus()
        {
            SavedStatus = Fields.SomeKeysDirty ? SavedEnum.Unsaved : SavedEnum.Saved;
        }

        private void UpdateType()
        {
            OnPropertyChanged(nameof(IsNormal));
            OnPropertyChanged(nameof(IsBuiltIn));

            UpdateApplyStatus();
            UpdateSavedStatus();
        }

        private void UpdateApplyStatus()
        {
            OnPropertyChanged(nameof(AppliedStatus));
            OnPropertyChanged(nameof(IsApplied));
            OnPropertyChanged(nameof(Status));
        }

        private void UpdateSavedStatus()
        {
            OnPropertyChanged(nameof(SavedStatus));
            OnPropertyChanged(nameof(IsSaved));
            OnPropertyChanged(nameof(IsUnsaved));
            OnPropertyChanged(nameof(Status));
        }
    }
}
