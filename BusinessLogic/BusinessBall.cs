//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        Data.IBall dataBall; 
        public Ball(Data.IBall ball)
        {
            dataBall = ball;
            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        public void Move(double delta)
        {
            dataBall.SetPosition(dataBall.GetPosition().x + delta * dataBall.GetVelocity().x, dataBall.GetPosition().y + delta * dataBall.GetVelocity().y);
            StayInBox(0, 399, 0, 399);
            NewPositionNotification?.Invoke(this, new Position(dataBall.GetPosition().x, dataBall.GetPosition().y));
        }

        #region private


        private void StayInBox(double left, double right, double Top, double Bottom)
        {
            if (dataBall.GetPosition().x + dataBall.Diameter / 2 > right)
            {
                dataBall.SetVelocity(-dataBall.GetVelocity().x, dataBall.GetVelocity().y);
                dataBall.SetPosition(dataBall.GetPosition().x - (dataBall.GetPosition().x + dataBall.Diameter / 2 - right) * 2, dataBall.GetPosition().y);
            }
            if (dataBall.GetPosition().x - dataBall.Diameter / 2 < left)
            {
                dataBall.SetVelocity(-dataBall.GetVelocity().x, dataBall.GetVelocity().y);
                dataBall.SetPosition(dataBall.GetPosition().x + (left - (dataBall.GetPosition().x - dataBall.Diameter / 2)) * 2, dataBall.GetPosition().y);
            }

            if (dataBall.GetPosition().y + dataBall.Diameter / 2 > Bottom)
            {
                dataBall.SetVelocity(dataBall.GetVelocity().x, -dataBall.GetVelocity().y);
                dataBall.SetPosition(dataBall.GetPosition().x, dataBall.GetPosition().y - (dataBall.GetPosition().y + dataBall.Diameter / 2 - Bottom) * 2);
            }

            if (dataBall.GetPosition().y - dataBall.Diameter / 2 < Top)
            {
                dataBall.SetVelocity(dataBall.GetVelocity().x, -dataBall.GetVelocity().y);
                dataBall.SetPosition(dataBall.GetPosition().x, dataBall.GetPosition().y + (Top - (dataBall.GetPosition().y - dataBall.Diameter / 2)) * 2);
            }

        }

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            Move(0.01);
            StayInBox(0, 399, 0, 399);
            NewPositionNotification?.Invoke(this, new Position(dataBall.GetPosition().x, dataBall.GetPosition().y));

        }

        #endregion private
    }
}