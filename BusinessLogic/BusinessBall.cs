//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Numerics;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        Data.IBall dataBall; 
        public Ball(Data.IBall ball)
        {
            dataBall = ball;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        public void Move(double delta, DiagnosticsLoggerAbstractAPI _logger)
        {
            dataBall.LockAll();
            try
            {
                dataBall.SetPosition(dataBall.GetPosition().x + delta * dataBall.GetVelocity().x, dataBall.GetPosition().y + delta * dataBall.GetVelocity().y);
                StayInBox(0, 399, 0, 399, _logger);
            }
            finally
            {
                dataBall.UnLockAll();
            }
        }

        public void CollideWithBalls(List<Ball> balls, DiagnosticsLoggerAbstractAPI _logger)
        {
            foreach (var otherBall in balls)
            {
                if (otherBall == this) continue;
                Ball first, second;
                if (this.GetHashCode() < otherBall.GetHashCode())
                {
                    first = this;
                    second = otherBall;
                }
                else
                {
                    first = otherBall;
                    second = this;
                }
                first.dataBall.LockAll();
                try
                {
                    second.dataBall.LockAll();
                    try 
                    { 
                        if (CollideWithBall(otherBall))
                        {
                            _logger.Log($"Ball Collided with ball. Ball1: Position: ({dataBall.GetPosition().x:F3}, {dataBall.GetPosition().y:F3}), Diamiter: {dataBall.Diameter} Ball2: Position: ({otherBall.dataBall.GetPosition().x:F3}, {otherBall.dataBall.GetPosition().y:F3}), Diamiter: {otherBall.dataBall.Diameter}");
                            HandleCollision(otherBall);
                        }                    
                    }
                    finally
                    {
                        second.dataBall.UnLockAll();
                    }
                }
                finally
                {
                    first.dataBall.UnLockAll();
                }
            }
            NewPositionNotification?.Invoke(this, new Position(dataBall.GetPosition().x, dataBall.GetPosition().y));
        }

        #region private

        private bool CollideWithBall(Ball otherBall)
        {
            double distance = Math.Sqrt(Math.Pow(dataBall.GetPosition().x - otherBall.dataBall.GetPosition().x, 2) + Math.Pow(dataBall.GetPosition().y - otherBall.dataBall.GetPosition().y, 2));
            return distance < (dataBall.Diameter + otherBall.dataBall.Diameter) / 2;
        }

        private void HandleCollision(Ball otherBall)
        {
            double masa1 = dataBall.Diameter * dataBall.Diameter;
            double masa2 = otherBall.dataBall.Diameter * otherBall.dataBall.Diameter;

            double odlegloscX = dataBall.GetPosition().x - otherBall.dataBall.GetPosition().x;
            double odlegloscY = dataBall.GetPosition().y - otherBall.dataBall.GetPosition().y;
            double odleglosc = Math.Sqrt(Math.Pow(odlegloscX, 2) + Math.Pow(odlegloscY, 2));

            double przesuniecie = (dataBall.Diameter + otherBall.dataBall.Diameter) / 2 - odleglosc;
            double przesuniecieX = przesuniecie * odlegloscX / odleglosc;
            double przesuniecieY = przesuniecie * odlegloscY / odleglosc;

            double odlegloscXNormalized = odlegloscX / odleglosc;
            double odlegloscYNormalized = odlegloscY / odleglosc;

            double predkoscNormalna1 = dataBall.GetVelocity().x * odlegloscXNormalized + dataBall.GetVelocity().y * odlegloscYNormalized;
            double predkoscNormalna2 = otherBall.dataBall.GetVelocity().x * odlegloscXNormalized + otherBall.dataBall.GetVelocity().y * odlegloscYNormalized;

            double nowaPredkoscNormalna1 = (predkoscNormalna1 * (masa1 - masa2) + 2 * masa2 * predkoscNormalna2) / (masa1 + masa2);
            double nowaPredkoscNormalna2 = (predkoscNormalna2 * (masa2 - masa1) + 2 * masa1 * predkoscNormalna1) / (masa1 + masa2);

            double predkoscTangensowa1X = dataBall.GetVelocity().x - predkoscNormalna1 * odlegloscXNormalized;
            double predkoscTangensowa1Y = dataBall.GetVelocity().y - predkoscNormalna1 * odlegloscYNormalized;
            double predkoscTangensowa2X = otherBall.dataBall.GetVelocity().x - predkoscNormalna2 * odlegloscXNormalized;
            double predkoscTangensowa2Y = otherBall.dataBall.GetVelocity().y - predkoscNormalna2 * odlegloscYNormalized;

            double nowaPredkosc1X = predkoscTangensowa1X + nowaPredkoscNormalna1 * odlegloscXNormalized;
            double nowaPredkosc1Y = predkoscTangensowa1Y + nowaPredkoscNormalna1 * odlegloscYNormalized;
            double nowaPredkosc2X = predkoscTangensowa2X + nowaPredkoscNormalna2 * odlegloscXNormalized;
            double nowaPredkosc2Y = predkoscTangensowa2Y + nowaPredkoscNormalna2 * odlegloscYNormalized;

            dataBall.SetVelocity(nowaPredkosc1X, nowaPredkosc1Y);
            otherBall.dataBall.SetVelocity(nowaPredkosc2X, nowaPredkosc2Y);

            dataBall.SetPosition(
                dataBall.GetPosition().x + przesuniecieX * masa2 / (masa1 + masa2),
                dataBall.GetPosition().y + przesuniecieY * masa2 / (masa1 + masa2)
            );
            otherBall.dataBall.SetPosition(
                otherBall.dataBall.GetPosition().x - przesuniecieX * masa1 / (masa1 + masa2),
                otherBall.dataBall.GetPosition().y - przesuniecieY * masa1 / (masa1 + masa2)
            );
        }

        private void StayInBox(double left, double right, double Top, double Bottom, DiagnosticsLoggerAbstractAPI _logger)
        {
            if (dataBall.GetPosition().x + dataBall.Diameter / 2 > right)
            {
                double x = dataBall.GetVelocity().x;
                double y = dataBall.GetVelocity().y;
                dataBall.SetVelocity(-x, y);
                dataBall.SetPosition(dataBall.GetPosition().x - (dataBall.GetPosition().x + dataBall.Diameter / 2 - right) * 2, dataBall.GetPosition().y);
                _logger.Log($"Ball Collided with wall. old Velocity: ({x:F3}, {y:F3}) , new Velocity: ({dataBall.GetVelocity().x:F3}, {dataBall.GetVelocity().y:F3})");
            }
            if (dataBall.GetPosition().x - dataBall.Diameter / 2 < left)
            {
                double x = dataBall.GetVelocity().x;
                double y = dataBall.GetVelocity().y;
                dataBall.SetVelocity(-x, y);
                dataBall.SetPosition(dataBall.GetPosition().x + (left - (dataBall.GetPosition().x - dataBall.Diameter / 2)) * 2, dataBall.GetPosition().y);
                _logger.Log($"Ball Collided with wall. old Velocity: ({x:F3}, {y:F3}) , new Velocity: ({dataBall.GetVelocity().x:F3}, {dataBall.GetVelocity().y:F3})");
            }

            if (dataBall.GetPosition().y + dataBall.Diameter / 2 > Bottom)
            {
                double x = dataBall.GetVelocity().x;
                double y = dataBall.GetVelocity().y;
                dataBall.SetVelocity(x, -y);
                dataBall.SetPosition(dataBall.GetPosition().x, dataBall.GetPosition().y - (dataBall.GetPosition().y + dataBall.Diameter / 2 - Bottom) * 2);
                _logger.Log($"Ball Collided with wall. old Velocity: ({x:F3}, {y:F3}) , new Velocity: ({dataBall.GetVelocity().x:F3}, {dataBall.GetVelocity().y:F3})");
            }

            if (dataBall.GetPosition().y - dataBall.Diameter / 2 < Top)
            {
                double x = dataBall.GetVelocity().x;
                double y = dataBall.GetVelocity().y;
                dataBall.SetVelocity(x, -y);
                dataBall.SetPosition(dataBall.GetPosition().x, dataBall.GetPosition().y + (Top - (dataBall.GetPosition().y - dataBall.Diameter / 2)) * 2);
                _logger.Log($"Ball Collided with wall. old Velocity: ({x:F3}, {y:F3}) , new Velocity: ({dataBall.GetVelocity().x:F3}, {dataBall.GetVelocity().y:F3})");
            }

        }


        #endregion private
    }
}