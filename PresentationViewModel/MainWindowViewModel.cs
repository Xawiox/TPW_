﻿//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
        }

        #endregion ctor

        #region public API

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
            Observer.Dispose();
        }

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer.Dispose();
                    ModelLayer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;

        #endregion private


        private int _numberOfBalls;
        public int NumberOfBalls
        {
            get { return _numberOfBalls; }
            set
            {
                if (value >= 0 && value <= 50 && _numberOfBalls != value)
                {
                    _numberOfBalls = value;
                    RaisePropertyChanged();
                    StartCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _isStartEnabled = true;
        public bool IsStartEnabled
        {
            get { return _isStartEnabled; }
            set
            {
                if (_isStartEnabled != value)
                {
                    _isStartEnabled = value;
                    RaisePropertyChanged();
                    StartCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private RelayCommand _startCommand;
        public RelayCommand StartCommand
        {
            get
            {
                if (_startCommand == null)
                {
                    _startCommand = new RelayCommand(
                        ExecuteStartCommand,
                        CanExecuteStartCommand);
                }
                return _startCommand;
            }
        }

        private bool CanExecuteStartCommand()
        {
            return NumberOfBalls > 0 && NumberOfBalls <= 50 && !Disposed && IsStartEnabled;
        }

        private void ExecuteStartCommand()
        {
            Start(NumberOfBalls);

            IsStartEnabled = false;
        }

    }
}