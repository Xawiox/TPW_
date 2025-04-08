//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor

    public DataImplementation()
    {
            //MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
            StartGameLoop();
        }

    #endregion ctor

    #region DataAbstractAPI

    public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));
      Random random = new Random();
      for (int i = 0; i < numberOfBalls; i++)
      {
            Vector startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
            double xVelocity = random.Next(-80, 80);
            double yVelocity = Math.Sqrt(6400 - xVelocity * xVelocity);
            double initialDiameter = random.Next(10, 40);
            Vector startingVelocity = new(xVelocity, yVelocity);
            Ball newBall = new(startingPosition, startingVelocity, initialDiameter);
            upperLayerHandler(startingPosition, newBall);
            BallsList.Add(newBall);
      }
    }

    #endregion DataAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          BallsList.Clear();
        }
        Disposed = true;
      }
      else
        throw new ObjectDisposedException(nameof(DataImplementation));
    }

    public override void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    //private bool disposedValue;
    private bool Disposed = false;

    private readonly Timer MoveTimer;
    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];

    private void Move(double deltaTime)
    {
      foreach (Ball item in BallsList)
        item.Move(new Vector(item.Velocity.x * deltaTime, item.Velocity.y * deltaTime));
    }

        private void StartGameLoop()
        {
            Task.Run(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                long lastUpdate = stopwatch.ElapsedMilliseconds;

                while (true)
                {
                    long now = stopwatch.ElapsedMilliseconds;
                    double deltaTime = (now - lastUpdate) / 1000.0;
                    lastUpdate = now;

                    Move(deltaTime);

                    Thread.Sleep(10);
                }
            });
        }

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
    internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
    {
      returnBallsList(BallsList);
    }

    [Conditional("DEBUG")]
    internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
    {
      returnNumberOfBalls(BallsList.Count);
    }

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    #endregion TestingInfrastructure
  }
}