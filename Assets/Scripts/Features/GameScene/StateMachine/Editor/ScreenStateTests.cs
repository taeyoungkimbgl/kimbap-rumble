using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scripts.Common.Core.StateMachine;
using Scripts.Features.GameScene.Header;
using Scripts.Features.GameScene.StateMachine.ScreenState;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.GameScene.StateMachine.Editor
{
    public class ScreenStateTests
    {
        private IObjectResolver _container;
        private StateMachineModel _model;
        private IStateMachineService _service;
        private StateMachinePresenter _presenter;

        [SetUp]
        public void SetUp()
        {
            var builder = new ContainerBuilder();
            StateMachineInstaller.Register(builder);
            builder.Register<HeaderModel>(Lifetime.Singleton);
            builder.Register<IHeaderService, HeaderServiceImpl>(Lifetime.Singleton);
            _container = builder.Build();
            _model = _container.Resolve<StateMachineModel>();
            _service = _container.Resolve<IStateMachineService>();
            _presenter = (StateMachinePresenter)_container.Resolve<IStartable>();

            // Synchronous EditMode tests invoke Start without advancing the PlayerLoop.
            _presenter.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _container?.Dispose();
            _container = null;
        }

        [Test]
        public void InstallerAndPresenterRegisterAllFiveScreenStatesInTheSameScope()
        {
            var states = new (ScreenStateType id, IState<ScreenStateType> state)[]
            {
                (ScreenStateType.Home, _container.Resolve<HomeScreenState>()),
                (ScreenStateType.Inventory, _container.Resolve<InventoryScreenState>()),
                (ScreenStateType.Album, _container.Resolve<AlbumScreenState>()),
                (ScreenStateType.PhotoDetail, _container.Resolve<PhotoDetailScreenState>()),
                (ScreenStateType.Result, _container.Resolve<ResultScreenState>()),
            };

            CollectionAssert.AreEquivalent(states.Select(entry => entry.state).ToArray(), _model.ScreenStates);
            CollectionAssert.AreEquivalent(
                Enum.GetValues(typeof(ScreenStateType)),
                _model.ScreenStates.Select(state => state.StateId).ToArray());
            Assert.IsNull(_model.CurrentScreenState);

            foreach (var (id, state) in states)
            {
                _service.ChangeScreenState(id);
                Assert.AreSame(state, _model.CurrentScreenState);
            }
        }

        [Test]
        public void ChangingScreenExitsBeforeUpdatingReferenceAndEnteringNextState()
        {
            var (home, inventory) = RegisterObservedStates();
            _service.ChangeScreenState(ScreenStateType.Home);
            var calls = new List<string>();
            home.Exited = () =>
            {
                Assert.AreSame(home, _model.CurrentScreenState);
                calls.Add("Home.Exit");
            };
            inventory.Entered = () =>
            {
                Assert.AreSame(inventory, _model.CurrentScreenState);
                calls.Add("Inventory.Enter");
            };

            _service.ChangeScreenState(ScreenStateType.Inventory);

            CollectionAssert.AreEqual(new[] { "Home.Exit", "Inventory.Enter" }, calls);
            Assert.AreEqual(1, home.EnterCount);
            Assert.AreEqual(1, home.ExitCount);
            Assert.AreEqual(1, inventory.EnterCount);
            Assert.AreEqual(0, inventory.ExitCount);
            Assert.AreSame(inventory, _model.CurrentScreenState);
        }

        [Test]
        public void RequestingCurrentScreenDoesNotReenterByDefault()
        {
            var (home, _) = RegisterObservedStates();
            _service.ChangeScreenState(ScreenStateType.Home);

            _service.ChangeScreenState(ScreenStateType.Home);

            Assert.AreSame(home, _model.CurrentScreenState);
            Assert.AreEqual(1, home.EnterCount);
            Assert.AreEqual(0, home.ExitCount);
        }

        [Test]
        public void ExplicitReentryExitsThenEntersCurrentScreenOnce()
        {
            var (home, _) = RegisterObservedStates();
            _service.ChangeScreenState(ScreenStateType.Home);
            var calls = new List<string>();
            home.Exited = () => calls.Add("Exit");
            home.Entered = () => calls.Add("Enter");

            _service.ChangeScreenState(ScreenStateType.Home, allowReentry: true);

            CollectionAssert.AreEqual(new[] { "Exit", "Enter" }, calls);
            Assert.AreSame(home, _model.CurrentScreenState);
            Assert.AreEqual(2, home.EnterCount);
            Assert.AreEqual(1, home.ExitCount);
        }

        [Test]
        public void UnregisteredDestinationLogsErrorAndKeepsCurrentScreen()
        {
            var (home, inventory) = RegisterObservedStates();
            _service.ChangeScreenState(ScreenStateType.Home);
            LogAssert.Expect(LogType.Error,
                "GameScene screen state is not registered. state=Album");

            _service.ChangeScreenState(ScreenStateType.Album);

            Assert.AreSame(home, _model.CurrentScreenState);
            Assert.AreEqual(1, home.EnterCount);
            Assert.AreEqual(0, home.ExitCount);
            Assert.AreEqual(0, inventory.EnterCount);
            Assert.AreEqual(0, inventory.ExitCount);
        }

        [Test]
        public void PresenterDisposalExitsCurrentScreenOnceAndClearsReference()
        {
            var (home, _) = RegisterObservedStates();
            _service.ChangeScreenState(ScreenStateType.Home);

            _presenter.Dispose();

            Assert.IsNull(_model.CurrentScreenState);
            Assert.AreEqual(1, home.ExitCount);

            _presenter.Dispose();

            Assert.IsNull(_model.CurrentScreenState);
            Assert.AreEqual(1, home.EnterCount);
            Assert.AreEqual(1, home.ExitCount);
        }

        [Test]
        public void PresenterDisposalBeforeEnteringAnyScreenDoesNotExitAState()
        {
            var (home, inventory) = RegisterObservedStates();

            _presenter.Dispose();
            _presenter.Dispose();

            Assert.IsNull(_model.CurrentScreenState);
            Assert.AreEqual(0, home.ExitCount);
            Assert.AreEqual(0, inventory.ExitCount);
        }

        [Test]
        public void ContainerDisposalInvokesPresenterExitAndClearsCurrentScreen()
        {
            var (home, _) = RegisterObservedStates();
            _service.ChangeScreenState(ScreenStateType.Home);

            _container.Dispose();
            _container = null;

            Assert.AreEqual(1, home.ExitCount);
            Assert.IsNull(_model.CurrentScreenState);
        }

        private (ObservedState home, ObservedState inventory) RegisterObservedStates()
        {
            _model.ScreenStates.Clear();
            var home = new ObservedState(ScreenStateType.Home);
            var inventory = new ObservedState(ScreenStateType.Inventory);
            _service.RegisterScreenState(home);
            _service.RegisterScreenState(inventory);
            return (home, inventory);
        }

        private sealed class ObservedState : IState<ScreenStateType>
        {
            public ScreenStateType StateId { get; }
            public int EnterCount { get; private set; }
            public int ExitCount { get; private set; }
            public Action Entered { get; set; }
            public Action Exited { get; set; }

            public ObservedState(ScreenStateType stateId)
            {
                StateId = stateId;
            }

            public void Enter()
            {
                EnterCount++;
                Entered?.Invoke();
            }

            public void Update() { }

            public void Exit()
            {
                ExitCount++;
                Exited?.Invoke();
            }
        }
    }
}
