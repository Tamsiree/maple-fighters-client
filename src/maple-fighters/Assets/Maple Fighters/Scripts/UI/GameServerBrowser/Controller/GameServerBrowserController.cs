﻿using System.Collections.Generic;
using System.Linq;
using UI.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.UI.GameServerBrowser
{
    [RequireComponent(typeof(GameServerBrowserInteractor))]
    public class GameServerBrowserController : MonoBehaviour,
                                               IOnConnectionFinishedListener,
                                               IOnGameServerReceivedListener
    {
        [SerializeField]
        private string sceneName;

        private GameServerBrowserInteractor gameServerBrowserInteractor;
        private IGameServerBrowserView gameServerBrowserView;
        private GameServerViewCollection? gameServerViewCollection;

        private void Awake()
        {
            gameServerBrowserInteractor =
                GetComponent<GameServerBrowserInteractor>();

            CreateAndSubscribeToGameServerBrowserWindow();
        }

        private void Start()
        {
            ShowGameServerBrowserWindow();
        }

        private void CreateAndSubscribeToGameServerBrowserWindow()
        {
            gameServerBrowserView = UIElementsCreator.GetInstance()
                .Create<GameServerBrowserWindow>();
            gameServerBrowserView.JoinButtonClicked +=
                OnJoinButtonClicked;
            gameServerBrowserView.RefreshButtonClicked +=
                OnRefreshButtonClicked;
        }

        private void OnDestroy()
        {
            UnsubscribeFromGameServerViews();
            UnsubscribeFromGameServerBrowserWindow();
        }

        private void UnsubscribeFromGameServerBrowserWindow()
        {
            if (gameServerBrowserView != null)
            {
                gameServerBrowserView.JoinButtonClicked -=
                    OnJoinButtonClicked;
                gameServerBrowserView.RefreshButtonClicked -=
                    OnRefreshButtonClicked;
            }
        }

        private void ShowGameServerBrowserWindow()
        {
            gameServerBrowserView?.Show();
        }

        public void OnConnectionFailed()
        {
            // TODO: Show notice
        }

        public void OnGameServerReceived(IEnumerable<UIGameServerButtonData> datas)
        {
            UnsubscribeFromGameServerViews();
            DestroyGameServerViews();

            var index = 0;
            var array = datas.ToArray();

            gameServerViewCollection = new GameServerViewCollection(array.Length);

            foreach (var data in array)
            {
                var gameServerView = CreateAndSubscribeToGameServerButton();
                gameServerView.SetGameServerButtonData(data);

                gameServerViewCollection?.Set(index, gameServerView);

                index++;
            }

            ShowGameServerList();
        }

        private GameServerButton CreateAndSubscribeToGameServerButton()
        {
            var gameServerButton = UIElementsCreator.GetInstance()
                .Create<GameServerButton>(
                    UILayer.Foreground,
                    UIIndex.End,
                    gameServerBrowserView.GameServerList);
            gameServerButton.ButtonClicked += OnGameServerButtonClicked;

            return gameServerButton;
        }

        private void DestroyGameServerViews()
        {
            var gameServerViewes = gameServerViewCollection?.GetAll();
            if (gameServerViewes != null)
            {
                foreach (var gameServerView in gameServerViewes)
                {
                    Destroy(gameServerView.GameObject);
                }
            }
        }

        private void UnsubscribeFromGameServerViews()
        {
            var gameServerViewes = gameServerViewCollection?.GetAll();
            if (gameServerViewes != null)
            {
                foreach (var gameServerView in gameServerViewes)
                {
                    if (gameServerView != null)
                    {
                        gameServerView.ButtonClicked -=
                            OnGameServerButtonClicked;
                    }
                }
            }
        }

        private void OnGameServerButtonClicked(string serverName)
        {
            gameServerBrowserView?.EnableJoinButton();

            // TODO: GameServerSelected(serverName)
        }

        private void OnJoinButtonClicked()
        {
            HideGameServerBrowserWindow();

            // TODO: Remove this from here
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

            // TODO: JoinGameServer()
        }

        private void HideGameServerBrowserWindow()
        {
            gameServerBrowserView?.DisableJoinButton();
            gameServerBrowserView?.DisableRefreshButton();
            gameServerBrowserView?.Hide();
        }

        private void OnRefreshButtonClicked()
        {
            ShowRefreshingGameServerList();

            gameServerBrowserInteractor.ProvideGameServers();
        }
        
        private void ShowGameServerList()
        {
            HideRefreshImage();

            gameServerBrowserView?.DisableJoinButton();
            gameServerBrowserView?.EnableRefreshButton();
        }

        private void HideRefreshImage()
        {
            gameServerBrowserView?.RefreshImage?.Hide();
        }

        private void ShowRefreshingGameServerList()
        {
            ShowRefreshImage();

            gameServerBrowserView?.DisableJoinButton();
            gameServerBrowserView?.DisableRefreshButton();
        }

        private void ShowRefreshImage()
        {
            gameServerBrowserView?.RefreshImage?.Show();
        }
    }
}