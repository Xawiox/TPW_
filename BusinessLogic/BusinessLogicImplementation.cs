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
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        private CancellationTokenSource _cancellationTokenSource = new();
        private Barrier _barrier;

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
            List<Ball> balls = new();
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
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                long lastUpdate = stopwatch.ElapsedMilliseconds;
                var token = _cancellationTokenSource.Token;
                while (!token.IsCancellationRequested)
                {
                    long now = stopwatch.ElapsedMilliseconds;
                    double deltaTime = (now - lastUpdate) / 1000.0;
                    lastUpdate = now;

                    newBall.Move(deltaTime);
                    _barrier.SignalAndWait();
                    Thread.Sleep(10);
                }
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