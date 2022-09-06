using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Calculator.ViewModels;
using Calculator.Views;
using System;
using System.Linq;
using Calculator.Core;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class CalculatorView : BaseGuiView
{
    [SerializeField] TMP_InputField _output;
    [SerializeField] List<Button> _digitButtons;
    [SerializeField] List<Button> _operationButtons;
    [SerializeField] Button _confirmButton;
    [SerializeField] Button _getResultButton;
    [SerializeField] Button _eraseButton;
    public CalculatorViewModel ViewModel { get { return (CalculatorViewModel)VM; } }

    protected override void OnViewModelChanged(ViewModel oldViewModel, ViewModel newViewModel)
    {
        base.OnViewModelChanged(oldViewModel, newViewModel);

        if (oldViewModel is CalculatorViewModel old)
        {
            old.Output.OnValueChanged -= OutputChanged;
            old.State.OnValueChanged -= StateChanged;
        }
        if (ViewModel != null)
        {
            ViewModel.Output.OnValueChanged += OutputChanged;
            ViewModel.State.OnValueChanged += StateChanged;
            ViewModel.Init();
        }
    }

    private void StateChanged(CalculatorState oldValue, CalculatorState newValue)
    {
        (newValue switch
        {
            FirstNumberState => (Action)SetNumberInputState,
            SecondNumberState => SetNumberInputState,
            OperatorState => SetOperationInputState,
            GetResultState => SetGetResultInputState,
            ResultState => SetEraseInputState,
            ExceptionState => SetExceptionState,
            _ => throw new NotImplementedException(),
        })();
    }

    private void OutputChanged(string oldValue, string newValue)
    {
        _output.text = newValue;
        if (!string.IsNullOrEmpty(ViewModel.UserInput)) _confirmButton.interactable = true;
    }

    private void Start()
    {
        var viewModelFactory = new CalculatorViewModelFactory();
        VM = viewModelFactory.Create() as ViewModel;

        InitializeInputs();
#if UNITY_ANDROID && !UNITY_EDITOR
        SetNotificationChannel();
#endif
    }
    private void InitializeInputs()
    {
        _confirmButton.onClick.AddListener(ViewModel.OnControlButtonClicked);
        _getResultButton.onClick.AddListener(ViewModel.OnControlButtonClicked);
        _eraseButton.onClick.AddListener(ViewModel.OnControlButtonClicked);
        foreach (var button in _digitButtons.Concat(_operationButtons).ToList<Button>())
        {
            button.onClick.AddListener(() => ViewModel.OnUserInput(button.name));
        }
    }

    #region Input State Setters
    private void SetNumberInputState()
    {
        foreach (var button in _digitButtons)
        {
            button.interactable = true;
        }
        foreach (var button in _operationButtons)
        {
            button.interactable = false;
        }
        _confirmButton.interactable = false;
        _eraseButton.interactable = false;
        _getResultButton.interactable = false;
    }

    private void SetOperationInputState()
    {
        foreach (var button in _digitButtons)
        {
            button.interactable = false;
        }
        foreach (var button in _operationButtons)
        {
            button.interactable = true;
        }
        _confirmButton.interactable = false;
        _eraseButton.interactable = false;
        _getResultButton.interactable = false;
    }

    private void SetGetResultInputState()
    {
        foreach (var button in _digitButtons)
        {
            button.interactable = false;
        }
        foreach (var button in _operationButtons)
        {
            button.interactable = false;
        }
        _confirmButton.interactable = false;
        _eraseButton.interactable = false;
        _getResultButton.interactable = true;
    }

    private void SetEraseInputState()
    {
        foreach (var button in _digitButtons)
        {
            button.interactable = false;
        }
        foreach (var button in _operationButtons)
        {
            button.interactable = false;
        }
        _confirmButton.interactable = false;
        _eraseButton.interactable = true;
        _getResultButton.interactable = false;
    }

    private void SetExceptionState()
    {
        SetEraseInputState();
#if UNITY_ANDROID && !UNITY_EDITOR
        ShowExceptionAlert("Get Premium to unlock this feature", int.Parse(ViewModel.FirstNumber));
#endif
    }
    #endregion

    #region Dispose
    private void OnDestroy()
    {
        DisposeInputs();
    }

    private void DisposeInputs()
    {
        _confirmButton.onClick.RemoveAllListeners();
        _getResultButton.onClick.RemoveAllListeners();
        _eraseButton.onClick.RemoveAllListeners();
        foreach (var button in _digitButtons.Concat(_operationButtons).ToList<Button>())
        {
            button.onClick.RemoveAllListeners();
        }
    }
    #endregion

    #region Android Alerts

#if UNITY_ANDROID && !UNITY_EDITOR
    private class OnClickListener : AndroidJavaProxy
    {
        public readonly Action Callback;
        public OnClickListener(Action callback) : base("android.content.DialogInterface$OnClickListener")
        {
            Callback = callback;
        }
        public void onClick(AndroidJavaObject dialog, int id)
        {
            Callback();
        }
    }

    private void ShowExceptionAlert(string content, int firstNumber)
    {
        AndroidJavaObject activity = null;
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            AndroidJavaObject dialog = null;
            using (AndroidJavaObject builder = new AndroidJavaObject("android.app.AlertDialog$Builder", activity))
            {
                builder.Call<AndroidJavaObject>("setMessage", content).Dispose();
                builder.Call<AndroidJavaObject>("setPositiveButton", "Get Premium", new OnClickListener(() => {
                    if (firstNumber > 0)
                    {
                        char inf = '\u221e';
                        ViewModel.Output.Value = $"{inf}";
                    } 
                    else if (firstNumber == 0)
                    {
                        char smiley = '\u30C4';
                        ViewModel.Output.Value = $"¯\\_({smiley})_/¯";
                    }
                    dialog.Dispose();
                    activity.Dispose();
                })).Dispose();
                builder.Call<AndroidJavaObject>("setNegativeButton", "Cancel", new OnClickListener(() => {
                    ShowNotification();
                    dialog.Dispose();
                    activity.Dispose();
                })).Dispose();
                dialog = builder.Call<AndroidJavaObject>("create");
            }
            dialog.Call("show");
        }));
    }
#endif

    #endregion

    #region Android Notifications
#if UNITY_ANDROID && !UNITY_EDITOR
    private void SetNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "my_notification_channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    private void ShowNotification()
    {
        var notification = new AndroidNotification();
        notification.Title = "No money - no honey";
        notification.FireTime = DateTime.Now;

        AndroidNotificationCenter.SendNotification(notification, "my_notification_channel_id");
    }
#endif

    #endregion
}
