using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Scripts.Common.Core.StateMachine;
using Scripts.Features.GameScene.StateMachine;
using Scripts.Features.GameScene.StateMachine.ScreenState;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Scripts.Features.GameScene.Header.Editor
{
    public class HeaderTests
    {
        private GameObject _root;
        private HeaderView _view;
        private TextMeshProUGUI _title;
        private Button _backButton;
        private HeaderModel _model;
        private IHeaderService _service;
        private HeaderPresenter _presenter;
        private IObjectResolver _container;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("HeaderTests");
            var canvasObject = new GameObject("CanvasHeader", typeof(RectTransform), typeof(Canvas));
            canvasObject.transform.SetParent(_root.transform, false);
            var titleObject = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObject.transform.SetParent(canvasObject.transform, false);
            _title = titleObject.GetComponent<TextMeshProUGUI>();
            var buttonObject = new GameObject("BackButton", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(canvasObject.transform, false);
            _backButton = buttonObject.GetComponent<Button>();

            var viewObject = new GameObject("HeaderView");
            viewObject.SetActive(false);
            viewObject.transform.SetParent(_root.transform, false);
            _view = viewObject.AddComponent<HeaderView>();
            var serialized = new SerializedObject(_view);
            serialized.FindProperty("_canvas").objectReferenceValue = canvasObject.GetComponent<Canvas>();
            serialized.FindProperty("_title").objectReferenceValue = _title;
            serialized.FindProperty("_backButton").objectReferenceValue = _backButton;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            // Exercise initialization synchronously in EditMode, without entering Play Mode.
            InvokeViewLifecycle("Awake");
            _model = new HeaderModel();
            _service = new HeaderServiceImpl(_model);
            _presenter = new HeaderPresenter(_model, _view);
        }

        [TearDown]
        public void TearDown()
        {
            _container?.Dispose();
            _container = null;
            _presenter?.Dispose();
            // The inactive test View does not receive Unity's normal destruction callback.
            if (_view != null)
                InvokeViewLifecycle("OnDestroy");
            Object.DestroyImmediate(_root);
        }

        [Test]
        public void InitialStateDisablesBackAndFirstRenderUsesSettingsAppliedBeforeStart()
        {
            Assert.AreEqual(string.Empty, _model.Title);
            Assert.IsFalse(_model.BackVisible);
            Assert.IsFalse(_model.InputEnabled);
            Assert.IsFalse(_backButton.gameObject.activeSelf);
            Assert.IsFalse(_backButton.interactable);
            var position = new Vector2(96, -8);
            _title.rectTransform.anchoredPosition = position;

            _service.Apply("PHOTO ALBUM", true);
            _service.SetInputEnabled(true);
            _presenter.Start();

            Assert.AreEqual("PHOTO ALBUM", _title.text);
            Assert.IsTrue(_backButton.gameObject.activeSelf);
            Assert.IsTrue(_backButton.interactable);
            _service.Apply("Kimbap Rumble", false);
            Assert.IsFalse(_backButton.gameObject.activeSelf);
            Assert.IsFalse(_backButton.interactable);
            Assert.AreEqual(position, _title.rectTransform.anchoredPosition);
        }

        [TestCase(false, false, 0)]
        [TestCase(false, true, 0)]
        [TestCase(true, false, 0)]
        [TestCase(true, true, 1)]
        public void BackRequestsRequireVisibleAndEnabledInput(bool visible, bool enabled, int expected)
        {
            var requests = 0;
            _service.BackRequested += () => requests++;
            _presenter.Start();
            _service.Apply("INVENTORY", visible);
            _service.SetInputEnabled(enabled);

            // Invoke even a disabled Button's event to check the Header notification boundary.
            _backButton.onClick.Invoke();

            Assert.AreEqual(expected, requests);
            Assert.AreEqual(visible, _backButton.gameObject.activeSelf);
            Assert.AreEqual(visible && enabled, _backButton.interactable);
        }

        [Test]
        public void RepeatedDisplayUpdatesDoNotMultiplyBackRequests()
        {
            var requests = 0;
            _service.BackRequested += () => requests++;
            _presenter.Start();
            for (var i = 0; i < 3; i++)
            {
                _service.Apply("INVENTORY", true);
                _service.SetInputEnabled(true);
                _backButton.onClick.Invoke();
                Assert.AreEqual(i + 1, requests);
            }
        }

        [Test]
        public void PresenterDisposalDisconnectsBothRenderingAndBackRequests()
        {
            var requests = 0;
            _service.BackRequested += () => requests++;
            _presenter.Start();
            _service.Apply("INVENTORY", true);
            _service.SetInputEnabled(true);

            _presenter.Dispose();
            _service.Apply("PHOTO ALBUM", true);
            _backButton.onClick.Invoke();

            Assert.AreEqual("INVENTORY", _title.text);
            Assert.AreEqual(0, requests);
        }

        [TestCase(ScreenStateType.Inventory, ScreenStateType.Home, "INVENTORY")]
        [TestCase(ScreenStateType.Album, ScreenStateType.Home, "PHOTO ALBUM")]
        [TestCase(ScreenStateType.PhotoDetail, ScreenStateType.Album, "PHOTO DETAIL")]
        [TestCase(ScreenStateType.PhotoDetail, ScreenStateType.Result, "PHOTO DETAIL")]
        public void ScreenReentryKeepsOneBackHandlerAndExitRemovesIt(
            ScreenStateType screen, ScreenStateType destination, string title)
        {
            var navigation = new RecordingStateMachineService();
            var stateModel = new StateMachineModel { PhotoDetailReturnScreen = destination };
            IState<ScreenStateType> state = screen switch
            {
                ScreenStateType.Inventory => new InventoryScreenState(_service, navigation),
                ScreenStateType.Album => new AlbumScreenState(_service, navigation),
                _ => new PhotoDetailScreenState(_service, navigation, stateModel),
            };

            for (var i = 0; i < 3; i++)
            {
                state.Enter();
                Assert.AreEqual(title, _model.Title);
                Assert.IsTrue(_model.BackVisible);
                Assert.IsFalse(_model.InputEnabled, "Screen display is not connected yet.");
                _model.RequestBack();
                Assert.AreEqual(i, navigation.Requests.Count);
                _service.SetInputEnabled(true);
                _model.RequestBack();
                Assert.AreEqual(i + 1, navigation.Requests.Count);
                Assert.AreEqual(destination, navigation.Requests[i]);

                state.Exit();
                Assert.IsFalse(_model.InputEnabled);
                // Simulate a later owner enabling the Header: the exited State must stay detached.
                _service.SetInputEnabled(true);
                _model.RequestBack();
                Assert.AreEqual(i + 1, navigation.Requests.Count);
            }
        }

        [Test]
        public void SharedScopeRendersStateSettingsAndDisposalDetachesItsHeader()
        {
            var builder = new ContainerBuilder();
            HeaderInstaller.Register(builder, _view);
            StateMachineInstaller.Register(builder);
            _container = builder.Build();
            foreach (var startable in _container.Resolve<IEnumerable<IStartable>>())
                startable.Start();
            var model = _container.Resolve<HeaderModel>();
            var header = _container.Resolve<IHeaderService>();
            var navigation = _container.Resolve<IStateMachineService>();
            var states = _container.Resolve<StateMachineModel>();

            navigation.ChangeScreenState(ScreenStateType.Home);
            Assert.AreEqual("Kimbap Rumble", _title.text);
            Assert.IsFalse(model.BackVisible);
            navigation.ChangeScreenState(ScreenStateType.Result);
            Assert.AreEqual("RUMBLE RESULT", _title.text);
            Assert.IsFalse(model.BackVisible);
            navigation.ChangeScreenState(ScreenStateType.Inventory);
            Assert.AreEqual("INVENTORY", _title.text);
            Assert.IsFalse(model.InputEnabled);
            header.SetInputEnabled(true);

            _container.Dispose();
            _container = null;
            Assert.IsNull(states.CurrentScreenState);
            Assert.IsFalse(model.InputEnabled);
            header.SetInputEnabled(true);
            model.RequestBack();
            Assert.IsNull(states.CurrentScreenState, "The exited Inventory must not request Home.");
            header.Apply("After disposal", true);
            Assert.AreEqual("INVENTORY", _title.text);
        }

        private void InvokeViewLifecycle(string method)
        {
            typeof(HeaderView).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(_view, null);
        }

        private sealed class RecordingStateMachineService : IStateMachineService
        {
            public List<ScreenStateType> Requests { get; } = new();

            public void ChangeScreenState(ScreenStateType type, bool allowReentry = false) => Requests.Add(type);
            public void RegisterGameState(IState<GameStateType> state) => throw new NotSupportedException();
            public void RegisterScreenState(IState<ScreenStateType> state) => throw new NotSupportedException();
            public void RegisterDetectionState(IState<DetectionStateType> state) => throw new NotSupportedException();
            public void ChangeGameState(GameStateType type, bool allowReentry = false) => throw new NotSupportedException();
            public void ChangeDetectionState(DetectionStateType type) => throw new NotSupportedException();
        }
    }
}
