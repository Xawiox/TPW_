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
using System.Numerics;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void MoveTestMethod()
        {
            VectorFixture fixturePosition = new(0.0, 0.0);
            double fixtureDiameter = 0.0;
            VectorFixture fixtureVelocity = new(0.0, 0.0);
            DataBallFixture dataBallFixture = new DataBallFixture(fixturePosition, fixtureVelocity, fixtureDiameter);
            Ball newInstance = new(dataBallFixture);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
            dataBallFixture.Move();
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
        }

        #region testing instrumentation

        private class DataBallFixture : Data.IBall
        {
            public DataBallFixture(VectorFixture position, VectorFixture velocity, double diameter)
            {
                Position = position;
                Velocity = velocity;
                Diameter = diameter;
            }

            public Data.IVector Velocity { get; set; }

            public double Diameter { get; init; } = 0;

            public IVector Position { get; set; }
            public event EventHandler<Data.IVector>? NewPositionNotification;

            internal void Move()
            {
                NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
            }
        }

        private class VectorFixture : Data.IVector
        {
            internal VectorFixture(double X, double Y)
            {
                x = X; y = Y;
            }

            public double x { get; init; }
            public double y { get; init; }
        }

        #endregion testing instrumentation

        [TestMethod]
        public void BallBounceTest()
        {
            TestVector initialPosition = new TestVector(395.0, 20.0);
            TestVector initialVelocity = new TestVector(20.0, 0.0);
            TestBall testBall = new TestBall
            {
                Position = initialPosition,
                Velocity = initialVelocity,
                Diameter = 10.0
            };

            Ball businessBall = new Ball(testBall);

            testBall.TriggerPositionChange();

            Assert.AreEqual(-20.0, testBall.Velocity.x);
            Assert.IsTrue(testBall.Position.x < 395.0);
        }

        public class TestVector : TP.ConcurrentProgramming.Data.IVector
        {
            public double x { get; init; }
            public double y { get; init; }

            public TestVector(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public class TestBall : TP.ConcurrentProgramming.Data.IBall
        {
            public event EventHandler<Data.IVector>? NewPositionNotification;
            public TP.ConcurrentProgramming.Data.IVector Velocity { get; set; }
            public TP.ConcurrentProgramming.Data.IVector Position { get; set; }
            public double Diameter { get; init; }

            public void SetVelocity(double x, double y)
            {
                Velocity = new TestVector(x, y);
            }

            public void SetPosition(double x, double y)
            {
                Position = new TestVector(x, y);
            }

            public void TriggerPositionChange()
            {
                NewPositionNotification?.Invoke(this, Position);
            }
        }
    }
}