//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using System.Threading;
using TP.ConcurrentProgramming.Data;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        private CancellationTokenSource _cancellationTokenSource = new();
        private DiagnosticsLogger _logger = new("./../../../../log.txt");
        private Barrier _barrier;
        private List<Ball> balls = new();

        public BusinessLogicImplementation() : this(null)
        { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            balls.Clear();
            _logger.Dispose();
            layerBellow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall, double> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            //layerBellow.Start(numberOfBalls, (startingPosition, databall) => upperLayerHandler(new Position(startingPosition.x, startingPosition.y), new Ball(databall), databall.Diameter));
            
            layerBellow.Start(numberOfBalls, (startingPosition, databall) =>
            {
                Ball newBall = new(databall);
                balls.Add(newBall);
                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), newBall, databall.Diameter);
            });
            _barrier = new Barrier(numberOfBalls);
            foreach (Ball ball in balls)
            {
                StartBall(ball);
            }
        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBellow;

        private void StartBall(Ball newBall)
        {
            
            Task.Run(() =>
            {
                _barrier.SignalAndWait();
                float framesPerSecond = 60;
                System.Timers.Timer timer = new(1000 / framesPerSecond);
                bool isBusy = false;
                timer.Elapsed += (sender, args) =>
                {
                    if (isBusy)
                    {
                        _logger.Log($"Real-Time Error");
                        return;
                    }
                    isBusy = true;
                   
                    newBall.Move(1.0 / framesPerSecond, _logger);
                    newBall.CollideWithBalls(balls, _logger);
                    isBusy = false;
                };
                timer.AutoReset = true;
                timer.Start();

            }, _cancellationTokenSource.Token);
        }

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}