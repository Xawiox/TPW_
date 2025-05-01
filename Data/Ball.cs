//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        #region ctor
        private ReaderWriterLockSlim _lockVelocity = new();
        private ReaderWriterLockSlim _lockPosition = new();

        internal Ball(Vector initialPosition, Vector initialVelocity, double initialDiameter)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
            Diameter = initialDiameter;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }
        public double Diameter { get; init; } = 0;


        public void SetVelocity(double x, double y)
        {
            _lockVelocity.EnterWriteLock();
            try
            {
                this.Velocity = new Vector(x, y);
            }
            finally
            {
                _lockVelocity.ExitWriteLock();
            }
        }

        public IVector GetVelocity()
        {
            _lockVelocity.EnterReadLock();
            try
            {
                return this.Velocity;
            }
            finally
            {
                _lockVelocity.ExitReadLock();
            }
        }

        public void SetPosition(double x, double y)
        {
            _lockPosition.EnterWriteLock();
            try
            {
                this.Position = new Vector(x, y);
            }
            finally
            {
                _lockPosition.ExitWriteLock();
            }
        }

        public IVector GetPosition()
        {
            _lockPosition.EnterReadLock();
            try
            {
                return this.Position;
            }
            finally
            {
                _lockPosition.ExitReadLock();
            }
        }
        #endregion IBall

        #region private

        public IVector Position { get; set; }

        //private void RaiseNewPositionChangeNotification()
        //{
        //    NewPositionNotification?.Invoke(this, Position);
        //}

        //internal void Move(double delta)
        //{
        //    //SetPosition(Position.x + delta * Velocity.x, Position.y + delta * Velocity.y);

        //    RaiseNewPositionChangeNotification();
        //}

        #endregion private
    }
}