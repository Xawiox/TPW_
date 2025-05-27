//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Drawing;

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
            ChangeColor  = false;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }
        public double Diameter { get; init; } = 0;

        public bool ChangeColor { get; set; } = false;
        public void LockAll()
        {
            _lockPosition.EnterWriteLock();
            _lockVelocity.EnterWriteLock();
        }

        public void UnLockAll()
        {
            _lockVelocity.ExitWriteLock();
            _lockPosition.ExitWriteLock();
        }

        public void SetVelocity(double x, double y)
        {
            this.Velocity = new Vector(x, y);
        }

        public IVector GetVelocity()
        {
            return this.Velocity;
        }

        public void SetPosition(double x, double y)
        {
            this.Position = new Vector(x, y);

        }

        public IVector GetPosition()
        {
            return this.Position;
        }

        public bool changeColor()
        {
            ChangeColor = !ChangeColor;
            return ChangeColor;
        }
        #endregion IBall

        #region private

        public IVector Position { get; set; }

        #endregion private
    }
}