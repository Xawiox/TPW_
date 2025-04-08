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
    public Ball(Data.IBall ball)
    {
      ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

        private void StayInBox(Data.IBall dataBall, double left, double right, double Top, double Bottom) {
            if (dataBall.Position.x + dataBall.Diameter / 2 > right)
            {
                dataBall.SetVelocity(-dataBall.Velocity.x, dataBall.Velocity.y);
                dataBall.SetPosition(dataBall.Position.x - (dataBall.Position.x + dataBall.Diameter/2 - right)*2, dataBall.Position.y);
            }
            if (dataBall.Position.x - dataBall.Diameter / 2 < left)
            {
                dataBall.SetVelocity(-dataBall.Velocity.x, dataBall.Velocity.y);
                dataBall.SetPosition(dataBall.Position.x + (left - (dataBall.Position.x - dataBall.Diameter / 2)) * 2, dataBall.Position.y);
            }
                
            if (dataBall.Position.y + dataBall.Diameter / 2 > Bottom)
            {
                dataBall.SetVelocity(dataBall.Velocity.x, -dataBall.Velocity.y);
                dataBall.SetPosition(dataBall.Position.x, dataBall.Position.y - (dataBall.Position.y + dataBall.Diameter / 2 - Bottom) * 2);
            }
                
            if (dataBall.Position.y - dataBall.Diameter/2 < Top)
            {
                dataBall.SetVelocity(dataBall.Velocity.x, -dataBall.Velocity.y);
                dataBall.SetPosition(dataBall.Position.x, dataBall.Position.y + (Top - (dataBall.Position.y - dataBall.Diameter / 2)) * 2);
            }
                
        }

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
                if (sender is Data.IBall ball)
                {
                    StayInBox(ball, 0, 399, 0, 399);
                    NewPositionNotification?.Invoke(this, new Position(ball.Position.x, ball.Position.y));
                }
                
        }

    #endregion private
  }
}