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
            newInstance.Move(1);
            Assert.AreNotEqual<IVector>(fixturePosition, dataBallFixture.Position);
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

            businessBall.Move(1);

            Assert.AreEqual(-20.0, testBall.Velocity.x);
            Assert.IsTrue(testBall.Position.x < 395.0);
        }

        [TestMethod]
        public void BallCollisionExistTest()
        {
            TestVector initialPosition1 = new TestVector(50.0, 20.0);
            TestVector initialPosition2 = new TestVector(80.0, 10.0);
            TestVector initialVelocity1 = new TestVector(15.0, 0.0);
            TestVector initialVelocity2 = new TestVector(-10.0, 10.0);
            TestBall testBall1 = new TestBall
            {
                Position = initialPosition1,
                Velocity = initialVelocity1,
                Diameter = 10.0
            };
            TestBall testBall2 = new TestBall
            {
                Position = initialPosition2,
                Velocity = initialVelocity2,
                Diameter = 10.0
            };

            Ball businessBall1 = new Ball(testBall1);
            Ball businessBall2 = new Ball(testBall2);

            businessBall1.Move(1);
            businessBall2.Move(1);
            businessBall1.CollideWithBalls(new List<Ball> { businessBall1,businessBall2 });
            businessBall2.CollideWithBalls(new List<Ball> { businessBall1,businessBall2 });

            Assert.IsTrue(testBall1.Velocity.x < 0.0);
            Assert.IsTrue(testBall1.Position.x < 65.0);
            Assert.IsTrue(testBall2.Velocity.x > 0.0);
            Assert.IsTrue(testBall2.Position.x > 70.0);
        }

        [TestMethod]
        public void BallCollisionMassIncludedTest()
        {
            TestVector initialPosition1 = new TestVector(60.0, 0.0);
            TestVector initialPosition2 = new TestVector(100.0, 0.0);
            TestVector initialVelocity1 = new TestVector(10.0, 0.0);
            TestVector initialVelocity2 = new TestVector(-10.0, 0.0);
            TestBall testBall1 = new TestBall
            {
                Position = initialPosition1,
                Velocity = initialVelocity1,
                Diameter = 10.0
            };
            TestBall testBall2 = new TestBall
            {
                Position = initialPosition2,
                Velocity = initialVelocity2,
                Diameter = 100.0
            };

            Ball businessBall1 = new Ball(testBall1);
            Ball businessBall2 = new Ball(testBall2);

            businessBall1.CollideWithBalls(new List<Ball> { businessBall1, businessBall2 });
            businessBall2.CollideWithBalls(new List<Ball> { businessBall1, businessBall2 });

            Assert.IsTrue(testBall1.Velocity.x < 0.0);
            Assert.IsTrue(testBall2.Velocity.x < 0.0);
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