

// Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Wave.MainWindow
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SynapseXtra;
using Wave;
using Wave.Classes.Cosmetic;
using Wave.Classes.Implementations;
using Wave.Classes.Passive;
using Wave.Classes.WebSockets;
using Wave.Controls;
using Wave.Controls.AIMessages;
using Wave.Controls.Settings;

namespace Wave;
public partial class MainWindow : Window
{
    public struct POINT
    {
        public int x;

        public int y;

        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct MINMAXINFO
    {
        public POINT ptReserved;

        public POINT ptMaxSize;

        public POINT ptMaxPosition;

        public POINT ptMinTrackSize;

        public POINT ptMaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

        public RECT rcMonitor;

        public RECT rcWork;

        public int dwFlags;
    }

    public struct RECT
    {
        public int left;

        public int top;

        public int right;

        public int bottom;

        public static readonly RECT Empty;

        public int Width => Math.Abs(right - left);

        public int Height => bottom - top;

        public bool IsEmpty
        {
            get
            {
                if (left < right)
                {
                    return top >= bottom;
                }
                return true;
            }
        }

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public RECT(RECT rcSrc)
        {
            left = rcSrc.left;
            top = rcSrc.top;
            right = rcSrc.right;
            bottom = rcSrc.bottom;
        }

        public override string ToString()
        {
            if (this == Empty)
            {
                return "RECT {Empty}";
            }
            return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect))
            {
                return false;
            }
            return this == (RECT)obj;
        }

        public override int GetHashCode()
        {
            return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
        }

        public static bool operator ==(RECT rect1, RECT rect2)
        {
            if (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right)
            {
                return rect1.bottom == rect2.bottom;
            }
            return false;
        }

        public static bool operator !=(RECT rect1, RECT rect2)
        {
            return !(rect1 == rect2);
        }
    }

    private readonly string fullscreenPath = "M200-200h80q17 0 28.5 11.5T320-160q0 17-11.5 28.5T280-120H160q-17 0-28.5-11.5T120-160v-120q0-17 11.5-28.5T160-320q17 0 28.5 11.5T200-280v80Zm560 0v-80q0-17 11.5-28.5T800-320q17 0 28.5 11.5T840-280v120q0 17-11.5 28.5T800-120H680q-17 0-28.5-11.5T640-160q0-17 11.5-28.5T680-200h80ZM200-760v80q0 17-11.5 28.5T160-640q-17 0-28.5-11.5T120-680v-120q0-17 11.5-28.5T160-840h120q17 0 28.5 11.5T320-800q0 17-11.5 28.5T280-760h-80Zm560 0h-80q-17 0-28.5-11.5T640-800q0-17 11.5-28.5T680-840h120q17 0 28.5 11.5T840-800v120q0 17-11.5 28.5T800-640q-17 0-28.5-11.5T760-680v-80Z";

    private readonly string exitFullscreenPath = "M240-240h-80q-17 0-28.5-11.5T120-280q0-17 11.5-28.5T160-320h120q17 0 28.5 11.5T320-280v120q0 17-11.5 28.5T280-120q-17 0-28.5-11.5T240-160v-80Zm480 0v80q0 17-11.5 28.5T680-120q-17 0-28.5-11.5T640-160v-120q0-17 11.5-28.5T680-320h120q17 0 28.5 11.5T840-280q0 17-11.5 28.5T800-240h-80ZM240-720v-80q0-17 11.5-28.5T280-840q17 0 28.5 11.5T320-800v120q0 17-11.5 28.5T280-640H160q-17 0-28.5-11.5T120-680q0-17 11.5-28.5T160-720h80Zm480 0h80q17 0 28.5 11.5T840-680q0 17-11.5 28.5T800-640H680q-17 0-28.5-11.5T640-680v-120q0-17 11.5-28.5T680-840q17 0 28.5 11.5T720-800v80Z";

    private WindowState previousWindowState;

    private bool isTransferredToUI;

    private int? selectedProcessId;

    private readonly Dictionary<string, int> notifications = new Dictionary<string, int>();

    private Storyboard currentNotification;

    private bool isNotifying;

    private int tabReferences;

    private readonly AIChat aiChat;

    private bool isAIPanelOpen = true;

    private int aiReferences;

    private bool isSearching;

    private string lastQuery;

    private int currentPage = 1;

    private int totalPages = 1;

    private Advertisement currentAdvertisement;

    private readonly System.Timers.Timer saveTabTimer = new System.Timers.Timer(60000.0);



    private void DoNotification()
    {
        isNotifying = true;
        KeyValuePair<string, int> keyValuePair = notifications.First();
        notifications.Remove(keyValuePair.Key);
        NotificationContent.Text = keyValuePair.Key;
        DurationIndicator.Width = 0.0;
        currentNotification = Animation.Animate(new AnimationPropertyBase(NotificationBorder)
        {
            Property = FrameworkElement.WidthProperty,
            To = 280
        }, new AnimationPropertyBase(DurationIndicator)
        {
            Property = FrameworkElement.WidthProperty,
            To = 278,
            Duration = new Duration(TimeSpan.FromMilliseconds(keyValuePair.Value)),
            DisableEasing = true
        });
        currentNotification.Completed += delegate
        {
            CloseNotification();
        };
    }

    private void CloseNotification()
    {
        Animation.Animate(new AnimationPropertyBase(NotificationBorder)
        {
            Property = FrameworkElement.WidthProperty,
            To = 0
        }, new AnimationPropertyBase(DurationIndicator)
        {
            Property = FrameworkElement.WidthProperty,
            To = 0
        }).Completed += async delegate
        {
            if (notifications.Count > 0)
            {
                await Task.Delay(250);
                DoNotification();
            }
            else
            {
                isNotifying = false;
            }
        };
    }

    private void PopupNotification(string message, int duration = 2500)
    {
        notifications.Add(message, duration);
        if (!isNotifying)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                DoNotification();
            });
        }
    }

    private void NewTab(string tabName = "x", string content = "print('Hello World!');", bool autoSave = true)
    {
        if (tabName == "x")
        {
            tabReferences++;
            tabName = "Tab " + tabReferences + ".lua";
        }
        TabItem tabItem = new TabItem
        {
            BorderThickness = new Thickness(0.0),
            FontFamily = new FontFamily("SF Pro"),
            FontSize = 14.0,
            Foreground = new SolidColorBrush(Colors.Gainsboro),
            Header = tabName,
            Height = 40.0,
            Margin = new Thickness(-2.0, -2.0, -2.0, 0.0)
        };
        ChromiumWebBrowser newBrowser = new ChromiumWebBrowser
        {
            BorderThickness = new Thickness(0.0)
        };
        newBrowser.FrameLoadEnd += async delegate
        {
            await Task.Delay(1000);
            newBrowser.ExecuteScriptAsync("\r\n                    window.setText('" + HttpUtility.JavaScriptStringEncode(content) + "');\r\n                    window.updateOptions({\r\n                        minimap: {\r\n                            enabled: " + Settings.Instance.ShowMinimap.ToString().ToLower() + "\r\n                        },\r\n                        inlayHints: {\r\n                            enabled: " + Settings.Instance.ShowInlayHints.ToString().ToLower() + "\r\n                        },\r\n                        fontSize: " + Settings.Instance.EditorTextSize + "\r\n                    });\r\n                ");
            newBrowser.GetBrowserHost().WindowlessFrameRate = Settings.Instance.EditorRefreshRate;
            if (Settings.Instance.SaveTabs && autoSave)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    AutoSaveTabs();
                });
            }
        };
        tabItem.Content = newBrowser;
        newBrowser.Load("http://localhost:1111");
        tabItem.IsSelected = true;
        ScriptTabs.Items.Add(tabItem);
    }

    private void RemoveTab(string tabName)
    {
        if (ScriptTabs.Items.Count <= 1)
        {
            return;
        }
        foreach (TabItem item in (IEnumerable)ScriptTabs.Items)
        {
            if ((string)item.Header == tabName)
            {
                ScriptTabs.Items.Remove(item);
                break;
            }
        }
        string path = "data/tabs/" + tabName;
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        ((TabItem)ScriptTabs.Items[0]).IsSelected = true;
    }

    private async void AutoSaveTabs()
    {
        string[] files = Directory.GetFiles("data/tabs");
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
        TabItem[] array = ScriptTabs.Items.Cast<TabItem>().ToArray();
        TabItem[] array2 = array;
        foreach (TabItem tabItem in array2)
        {
            ChromiumWebBrowser chromiumWebBrowser = (ChromiumWebBrowser)tabItem.Content;
            if (chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
            {
                object result = (await chromiumWebBrowser.EvaluateScriptAsync("window.getText();")).Result;
                if (result != null)
                {
                    File.WriteAllText(Environment.CurrentDirectory + "/data/tabs/" + tabItem.Header, result.ToString());
                }
            }
        }
    }

    private async void SendAIMessage(string message)
    {
        PopupNotification("This is just a preview, AI is disabled for now ;)", 4000);
    }

    private string GetApiLink(string query, int page)
    {
        if (query == "")
        {
            return "https://scriptblox.com/api/script/fetch?page=" + page;
        }
        // https://scriptblox.com/api/script/search?q=bloxfruits&page=2
        return "https://scriptblox.com/api/script/search" + "?q=" + query + "&page=" + page;
    }

    private void SetCurrentPage(int page)
    {
        currentPage = page;
        CurrentPageLabel.Content = "Page " + page + "/" + totalPages;
    }

    private async void SearchScripts(string query = "", int page = 1)
    {
        if (isSearching)
        {
            return;
        }
        isSearching = true;
        lastQuery = query;
        try
        {
            SearchResultPanel.Children.Clear();
            HttpClient client = new HttpClient();
            try
            {
                JToken jToken = JToken.Parse(await (await client.GetAsync(GetApiLink(query, page))).Content.ReadAsStringAsync())["result"];
                ScriptObject[] array = JsonConvert.DeserializeObject<ScriptObject[]>(jToken["scripts"].ToString());
                foreach (ScriptObject scriptObject in array)
                {
                    ScriptResult scriptResult = new ScriptResult(scriptObject);
                    ((Button)scriptResult.FindName("ExecuteButton")).Click += async delegate
                    {
                        string script = await scriptObject.GetScript();
                        if (InstanceStack.Children.Count > 1)
                        {
                            foreach (InstancePanel child in InstanceStack.Children)
                            {
                                child.Script = script;
                            }
                            AnimatePopupIn(InstancePopupGrid);
                        }
                        else
                        {
                            Roblox.ExecuteAll(script);
                        }
                    };
                    SearchResultPanel.Children.Add(scriptResult);
                }
                totalPages = Math.Max((int)jToken["totalPages"], 1);
            }
            finally
            {
                ((IDisposable)client)?.Dispose();
            }
            SetCurrentPage(page);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            isSearching = false;
        }
    }

    private void ShowAdPane()
    {
        if (AdBorder.Visibility == Visibility.Collapsed)
        {
            MainBorder.Margin = new Thickness(4.0, 4.0, 4.0, 124.0);
            base.Height += 120.0;
            AdBorder.Visibility = Visibility.Visible;
        }
        base.MinHeight = 570.0;
    }

    private void HideAdPane()
    {
        base.MinHeight = 450.0;
        if (AdBorder.Visibility == Visibility.Visible)
        {
            AdBorder.Visibility = Visibility.Collapsed;
            MainBorder.Margin = new Thickness(4.0, 4.0, 4.0, 4.0);
            base.Height -= 120.0;
        }
    }

    private void LoadAd()
    {
        currentAdvertisement = Advertisements.GetAdvertisement();
        AdBorder.Background = new ImageBrush
        {
            ImageSource = new BitmapImage(new Uri("pack://application:,,,/Wave;component" + currentAdvertisement.localImageLink, UriKind.Absolute))
        };
        ShowAdPane();
    }

    private void InitializeAds()
    {
        LoadAd();
        System.Timers.Timer timer = new System.Timers.Timer(300000.0);
        timer.Elapsed += delegate
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                LoadAd();
            });
        };
        timer.Start();
    }

    private Storyboard AnimatePopupIn(Grid popupGrid)
    {
        popupGrid.Opacity = 0.0;
        popupGrid.Visibility = Visibility.Visible;
        Storyboard result = Animation.Animate(new AnimationPropertyBase(popupGrid)
        {
            Property = UIElement.OpacityProperty,
            To = 1
        });
        MainBlur.BeginAnimation(BlurEffect.RadiusProperty, new DoubleAnimation
        {
            To = 8.0,
            Duration = TimeSpan.FromSeconds(0.35)
        });
        return result;
    }

    private Storyboard AnimatePopupOut(Grid popupGrid)
    {
        Storyboard storyboard = Animation.Animate(new AnimationPropertyBase(popupGrid)
        {
            Property = UIElement.OpacityProperty,
            To = 0
        });
        MainBlur.BeginAnimation(BlurEffect.RadiusProperty, new DoubleAnimation
        {
            To = 0.0,
            Duration = TimeSpan.FromSeconds(0.35)
        });
        storyboard.Completed += delegate
        {
            popupGrid.Visibility = Visibility.Collapsed;
        };
        return storyboard;
    }

    private async void InitiateLogin()
    {
        if (await ValidateLogin())
        {
            TransferToUI();
        }
        else
        {
            AnimatePopupIn(LoginPopupGrid);
        }
    }

    private async Task<bool> ValidateLogin()
    {
        return true;
    }

    private void TransferToUI()
    {
        LoginPopupGrid.Visibility = Visibility.Collapsed;
        WebSocketCollection.AddSocket("clientInformation", 6001).AddBehaviour<ClientInformationBehavior>("/transferClientInformation");
        foreach (KeyValuePair<string, WebSocket> socket in WebSocketCollection.Sockets)
        {
            socket.Value.Server.Start();
        }
        Roblox.OnProcessFound += delegate (object sender2, RobloxInstanceEventArgs e2)
        {
            if (!e2.AlreadyOpen)
            {
                PopupNotification("Attaching...");
            }
        };
        Roblox.OnProcessAdded += delegate (object sender2, RobloxInstanceEventArgs e2)
        {
            if (!e2.AlreadyOpen)
            {
                PopupNotification("Attached!");
            }
            Application.Current.Dispatcher.Invoke((Func<Task>)async delegate
            {
                InstancePanel instancePanel = new InstancePanel
                {
                    Margin = new Thickness(4.0, 2.0, 4.0, 2.0),
                    Width = double.NaN,
                    ProcessID = e2.Id
                };
                Task.Run(async delegate
                {
                    InstancePanel instancePanel2 = instancePanel;
                    instancePanel2.Script = (await ((ChromiumWebBrowser)ScriptTabs.SelectedContent).EvaluateScriptAsync("window.getText();")).Result.ToString();
                });
                ((Button)instancePanel.FindName("SelectButton")).Click += delegate
                {
                    UpdateSelectedProcessID(e2.Id, instancePanel.Username);
                };
                InstanceStack.Children.Add(instancePanel);
                if (InstanceStack.Children.Count == 1)
                {
                    while (instancePanel.Username == "Username")
                    {
                        await Task.Delay(250);
                    }
                    UpdateSelectedProcessID(e2.Id, instancePanel.Username);
                }
            });
        };
        Roblox.OnProcessRemoved += delegate (object sender2, RobloxInstanceEventArgs e2)
        {
            foreach (InstancePanel child in InstanceStack.Children)
            {
                if (child.ProcessID == e2.Id)
                {
                    InstanceStack.Children.Remove(child);
                    break;
                }
            }
            if (InstanceStack.Children.Count == 0)
            {
                UpdateSelectedProcessID(null, null);
            }
            else if (selectedProcessId == e2.Id)
            {
                UpdateSelectedProcessID(((InstancePanel)InstanceStack.Children[0]).ProcessID, ((InstancePanel)InstanceStack.Children[0]).Username);
            }
        };
        Roblox.OnProcessInformationGained += delegate (object sender2, DetailedRobloxInstanceEventArgs e2)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                foreach (InstancePanel child2 in InstanceStack.Children)
                {
                    if (child2.ProcessID == e2.ProcessId)
                    {
                        child2.Username = e2.Username;
                        child2.UserID = e2.UserId;
                    }
                }
            });
        };
        Roblox.Start();
        MultiInstance.IsChecked = Settings.Instance.MultiInstance;
        AutoExecute.IsChecked = Settings.Instance.AutoExecute;
        SaveTabs.IsChecked = Settings.Instance.SaveTabs;
        TopMost.IsChecked = Settings.Instance.TopMost;
        EditorRefreshRate.Value = Settings.Instance.EditorRefreshRate;
        EditorTextSize.Value = Settings.Instance.EditorTextSize;
        ShowMinimap.IsChecked = Settings.Instance.ShowMinimap;
        ShowInlayHints.IsChecked = Settings.Instance.ShowInlayHints;
        UseConversationHistory.IsChecked = Settings.Instance.UseConversationHistory;
        AppendCurrentScript.IsChecked = Settings.Instance.AppendCurrentScript;
        saveTabTimer.Elapsed += delegate
        {
            if (Settings.Instance.SaveTabs)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    AutoSaveTabs();
                });
            }
        };
        string[] allowedExtensions = new string[3] { ".lua", ".luau", ".txt" };
        string[] array = (from s in Directory.GetFiles("data/tabs")
                          where allowedExtensions.Contains(System.IO.Path.GetExtension(s).ToLowerInvariant())
                          select s).ToArray();
        if (array.Length != 0)
        {
            string[] array2 = array;
            foreach (string path in array2)
            {
                string fileName = System.IO.Path.GetFileName(path);
                Match match = Regex.Match(fileName, "Tab (\\d+).lua");
                if (match.Success)
                {
                    int num2 = int.Parse(match.Groups[1].Value);
                    if (num2 > tabReferences)
                    {
                        tabReferences = num2;
                    }
                }
                Match match2 = Regex.Match(fileName, "AI Reference (\\d+).lua");
                if (match2.Success)
                {
                    int num3 = int.Parse(match2.Groups[1].Value);
                    if (num3 > aiReferences)
                    {
                        aiReferences = num3;
                    }
                }
                NewTab(fileName, File.ReadAllText(path), autoSave: false);
            }
        }
        else
        {
            NewTab();
        }
        InitializeAds();
        SearchScripts();
        base.Topmost = TopMost.IsChecked;
        isTransferredToUI = true;
    }

    public override void OnApplyTemplate()
    {
        HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(WindowProc);
    }

    private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == 36)
        {
            WmGetMinMaxInfo(hwnd, lParam);
            handled = false;
        }
        return (IntPtr)0;
    }

    private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
        MINMAXINFO structure = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
        int flags = 2;
        IntPtr intPtr = MonitorFromWindow(hwnd, flags);
        if (intPtr != IntPtr.Zero)
        {
            MONITORINFO mONITORINFO = new MONITORINFO();
            GetMonitorInfo(intPtr, mONITORINFO);
            RECT rcWork = mONITORINFO.rcWork;
            RECT rcMonitor = mONITORINFO.rcMonitor;
            structure.ptMaxPosition.x = Math.Abs(rcWork.left - rcMonitor.left);
            structure.ptMaxPosition.y = Math.Abs(rcWork.top - rcMonitor.top);
            structure.ptMaxSize.x = Math.Abs(rcWork.right - rcWork.left);
            structure.ptMaxSize.y = Math.Abs(rcWork.bottom - rcWork.top);
        }
        Marshal.StructureToPtr(structure, lParam, fDeleteOld: true);
    }

    [DllImport("user32")]
    internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(ref Point lpPoint);

    [DllImport("User32")]
    internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

    private void UpdateSelectedProcessID(int? id, string username)
    {
        selectedProcessId = id;
        SelectedProcessButton.Content = username;
        SelectedProcessButton.Visibility = ((!id.HasValue) ? Visibility.Collapsed : Visibility.Visible);
    }

    public MainWindow()
    {
        MainWindow mainWindow = this;
        InitializeComponent();
        previousWindowState = base.WindowState;
        InstancePopupGrid.Visibility = Visibility.Collapsed;
        SelectedProcessButton.Visibility = Visibility.Collapsed;
        KeyValuePair<TabCheckBox, Grid>[] tabCheckBoxes = new KeyValuePair<TabCheckBox, Grid>[4]
        {
            new KeyValuePair<TabCheckBox, Grid>(HomeButton, HomeTab),
            new KeyValuePair<TabCheckBox, Grid>(EditorButton, EditorTab),
            new KeyValuePair<TabCheckBox, Grid>(ScriptsButton, ScriptsTab),
            new KeyValuePair<TabCheckBox, Grid>(SettingsButton, SettingsTab)
        };
        KeyValuePair<TabCheckBox, Grid>[] array = tabCheckBoxes;
        for (int i = 0; i < array.Length; i++)
        {
            KeyValuePair<TabCheckBox, Grid> tab = array[i];
            tab.Value.Visibility = Visibility.Collapsed;
            TabCheckBox checkBox = tab.Key;
            checkBox.OnEnabled += delegate
            {
                checkBox.CanToggle = false;
                tab.Value.Visibility = Visibility.Visible;
                KeyValuePair<TabCheckBox, Grid>[] array3 = tabCheckBoxes;
                foreach (KeyValuePair<TabCheckBox, Grid> keyValuePair in array3)
                {
                    TabCheckBox key = keyValuePair.Key;
                    if (key.Enabled && key != checkBox)
                    {
                        key.CanToggle = true;
                        key.Enabled = false;
                    }
                }
            };
            checkBox.OnDisabled += delegate
            {
                tab.Value.Visibility = Visibility.Collapsed;
            };
        }
        KeyValuePair<Button, Grid>[] array2 = new KeyValuePair<Button, Grid>[3]
        {
            new KeyValuePair<Button, Grid>(ExecutorHeaderButton, ExecutorHeader),
            new KeyValuePair<Button, Grid>(EditorHeaderButton, EditorHeader),
            new KeyValuePair<Button, Grid>(AIHeaderButton, AIHeader)
        };
        for (int i = 0; i < array2.Length; i++)
        {
            KeyValuePair<Button, Grid> settingsCategory = array2[i];
            settingsCategory.Key.Click += delegate
            {
                mainWindow.SettingsScroll.ScrollToVerticalOffset(settingsCategory.Value.TranslatePoint(default(Point), mainWindow.SettingsStack).Y);
            };
        }
        LoginPopupGrid.Opacity = 0.0;
        MainBlur.Radius = 0.0;
        NotificationBorder.Width = 0.0;
        HideAdPane();
        tabCheckBoxes[1].Key.Enabled = true;
        aiChat = new AIChat();
    }

    private void HomeWindow_Loaded(object sender, RoutedEventArgs e)
    {
        InitiateLogin();
        PopupNotification("NK is so HOT - mezavy", 10000);
    }

    private void HomeWindow_Closing(object sender, CancelEventArgs e)
    {
        if (Settings.Instance.SaveTabs)
        {
            AutoSaveTabs();
        }
    }

    private void HomeWindow_SourceInitialized(object sender, EventArgs e)
    {
        HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(WindowProc);
    }

    private void HomeWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            try
            {
                DragMove();
            }
            catch
            {
            }
        }
    }

    private void HomeWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (base.WindowState != previousWindowState)
        {
            previousWindowState = base.WindowState;
            MaximiseButton.Content = ((base.WindowState == WindowState.Maximized) ? exitFullscreenPath : fullscreenPath);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }

    private void MaximiseButton_Click(object sender, RoutedEventArgs e)
    {
        base.WindowState = ((base.WindowState != WindowState.Maximized) ? WindowState.Maximized : WindowState.Normal);
    }

    private void MinimiseButton_Click(object sender, RoutedEventArgs e)
    {
        base.WindowState = WindowState.Minimized;
    }

    private void NewTabButton_Click(object sender, RoutedEventArgs e)
    {
        NewTab();
    }

    private void CloseTabButton_Click(object sender, RoutedEventArgs e)
    {
        TabItem tabItem = (TabItem)((Button)sender).TemplatedParent;
        RemoveTab((string)tabItem.Header);
    }

    private void TabHeaderScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (e.ExtentWidthChange > 0.0)
        {
            ((ScrollViewer)sender).ScrollToRightEnd();
        }
    }

    private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
    {
        if (selectedProcessId.HasValue)
        {
            int[] processIds = new int[1] { selectedProcessId.Value };
            Roblox.ExecuteSpecific(processIds, (await ((ChromiumWebBrowser)ScriptTabs.SelectedContent).EvaluateScriptAsync("window.getText();")).Result.ToString());
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        ((ChromiumWebBrowser)ScriptTabs.SelectedContent).ExecuteScriptAsync("window.setText('');");
    }

    private void OpenFileButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Wave - Open Script",
            Filter = "Lua Script|*.lua|Text File|*.txt"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            NewTab(System.IO.Path.GetFileName(openFileDialog.FileName), File.ReadAllText(openFileDialog.FileName));
        }
    }

    private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
    {
        Encoding uTF = Encoding.UTF8;
        byte[] bytes = uTF.GetBytes((await ((ChromiumWebBrowser)ScriptTabs.SelectedContent).EvaluateScriptAsync("window.getText();")).Result.ToString());
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Title = "Wave - Save Script",
            Filter = "Lua Script|*.lua"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            FileStream obj = (FileStream)saveFileDialog.OpenFile();
            obj.Write(bytes, 0, bytes.Length);
            obj.Close();
        }
    }

    private void AIToggleButton_Click(object sender, RoutedEventArgs e)
    {
        isAIPanelOpen = !isAIPanelOpen;
        Animation.Animate(new AnimationPropertyBase(AIChatScroll)
        {
            Property = FrameworkElement.WidthProperty,
            To = (isAIPanelOpen ? 216 : 0)
        }, new AnimationPropertyBase(AIInputGrid)
        {
            Property = FrameworkElement.WidthProperty,
            To = (isAIPanelOpen ? 216 : 0)
        });
    }

    private void AIInput_GotFocus(object sender, RoutedEventArgs e)
    {
        AIInput.Text = "";
    }

    private void AIInput_LostFocus(object sender, RoutedEventArgs e)
    {
        AIInput.Text = "Send a message...";
    }

    private void AIInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return)
        {
            if (AIInput.Text.Length > 0)
            {
                SendAIMessage(AIInput.Text);
            }
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(AIInput), null);
            Keyboard.ClearFocus();
            AIInput.Text = "Send a message...";
        }
    }

    private void SearchQuery_GotFocus(object sender, RoutedEventArgs e)
    {
        SearchQuery.Text = "";
    }

    private void SearchQuery_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return)
        {
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(SearchQuery), null);
            Keyboard.ClearFocus();
            SearchScripts(SearchQuery.Text);
            if (SearchQuery.Text.Length == 0)
            {
                SearchQuery.Text = "Enter your search query...";
            }
        }
    }

    private void SearchQuery_LostFocus(object sender, RoutedEventArgs e)
    {
        if (SearchQuery.Text.Length == 0)
        {
            SearchQuery.Text = "Enter your search query...";
        }
    }

    private void SearchBtn_Click(object sender, RoutedEventArgs e)
    {
        SearchScripts(SearchQuery.Text);
        if (SearchQuery.Text.Length == 0)
        {
            SearchQuery.Text = "Enter your search query...";
        }
    }

    private void NextPageBtn_Click(object sender, RoutedEventArgs e)
    {
        if (currentPage < totalPages)
        {
            SearchScripts(lastQuery, currentPage + 1);
        }
    }

    private void PreviousPageBtn_Click(object sender, RoutedEventArgs e)
    {
        if (currentPage > 1)
        {
            SearchScripts(lastQuery, currentPage - 1);
        }
    }

    private void ScriptBloxCredits_Click(object sender, RoutedEventArgs e)
    {
        Process.Start("https://scriptblox.com");
    }

    private void CloseNotificationButton_Click(object sender, RoutedEventArgs e)
    {
        if (currentNotification.GetCurrentState() == ClockState.Active)
        {
            currentNotification.Stop();
            CloseNotification();
        }
    }

    private void CloseAdButton_Click(object sender, RoutedEventArgs e)
    {
        HideAdPane();
    }

    private void AdBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Process.Start(currentAdvertisement.redirectLink);
    }

    private void MultiInstance_OnCheckedChanged(object sender, EventArgs e)
    {
        Settings.Instance.MultiInstance = MultiInstance.IsChecked;
        Settings.Instance.Save();
        Bloxstrap.Instance.MultiInstanceLaunching = MultiInstance.IsChecked;
        Bloxstrap.Instance.Save();
    }

    private void AutoExecute_OnCheckedChanged(object sender, EventArgs e)
    {
        Settings.Instance.AutoExecute = AutoExecute.IsChecked;
        Settings.Instance.Save();
    }

    private void SaveTabs_OnCheckedChanged(object sender, EventArgs e)
    {
        Settings.Instance.SaveTabs = SaveTabs.IsChecked;
        Settings.Instance.Save();
        if (!isTransferredToUI)
        {
            return;
        }
        if (SaveTabs.IsChecked)
        {
            AutoSaveTabs();
            saveTabTimer.Start();
            return;
        }
        saveTabTimer.Stop();
        string[] files = Directory.GetFiles("data/tabs");
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
    }

    private void EditorRefreshRate_OnValueChanged(object sender, EventArgs e)
    {
        Settings.Instance.EditorRefreshRate = (int)EditorRefreshRate.Value;
        Settings.Instance.Save();
        foreach (TabItem item in (IEnumerable)ScriptTabs.Items)
        {
            ((ChromiumWebBrowser)item.Content).GetBrowserHost().WindowlessFrameRate = Settings.Instance.EditorRefreshRate;
        }
    }

    private void EditorTextSize_OnValueChanged(object sender, EventArgs e)
    {
        Settings.Instance.EditorTextSize = EditorTextSize.Value;
        Settings.Instance.Save();
        foreach (TabItem item in (IEnumerable)ScriptTabs.Items)
        {
            ((ChromiumWebBrowser)item.Content).ExecuteScriptAsync("window.updateOptions({ fontSize: " + Settings.Instance.EditorTextSize + " });");
        }
    }

    private void ShowMinimap_OnCheckedChanged(object sender, EventArgs e)
    {
        Settings.Instance.ShowMinimap = ShowMinimap.IsChecked;
        Settings.Instance.Save();
        foreach (TabItem item in (IEnumerable)ScriptTabs.Items)
        {
            ((ChromiumWebBrowser)item.Content).ExecuteScriptAsync("window.updateOptions({ minimap: { enabled: " + Settings.Instance.ShowMinimap.ToString().ToLower() + "} });");
        }
    }

    private void ShowInlayHints_OnCheckedChanged(object sender, EventArgs e)
    {
        Settings.Instance.ShowInlayHints = ShowInlayHints.IsChecked;
        Settings.Instance.Save();
        foreach (TabItem item in (IEnumerable)ScriptTabs.Items)
        {
            ((ChromiumWebBrowser)item.Content).ExecuteScriptAsync("window.updateOptions({ inlayHints: { enabled: " + Settings.Instance.ShowInlayHints.ToString().ToLower() + "} });");
        }
    }

    private void UseConversationHistory_OnCheckedChanged(object sender, EventArgs e)
    {
        Settings.Instance.UseConversationHistory = UseConversationHistory.IsChecked;
        Settings.Instance.Save();
    }

    private void TopMost_OnCheckedChanged(object sender, EventArgs e)
    {
        Settings.Instance.TopMost = TopMost.IsChecked;
        Settings.Instance.Save();
        base.Topmost = TopMost.IsChecked;
    }

    private void AppendCurrentScript_OnCheckedChanged(object sender, EventArgs e)
    {
        Settings.Instance.AppendCurrentScript = AppendCurrentScript.IsChecked;
        Settings.Instance.Save();
    }

    private void ClearConversationHistory_OnClicked(object sender, EventArgs e)
    {
        AIChatPanel.Children.RemoveRange(1, AIChatPanel.Children.Count - 1);
    }

    private void AIChatScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (e.ExtentHeightChange > 0.0)
        {
            AIChatScroll.ScrollToBottom();
        }
    }

    private void CloseInstanceButton_Click(object sender, RoutedEventArgs e)
    {
        AnimatePopupOut(InstancePopupGrid);
    }

    private async void SelectedProcessButton_Click(object sender, RoutedEventArgs e)
    {
        string script = (await ((ChromiumWebBrowser)ScriptTabs.SelectedContent).EvaluateScriptAsync("window.getText();")).Result.ToString();
        foreach (InstancePanel child in InstanceStack.Children)
        {
            child.Script = script;
        }
        AnimatePopupIn(InstancePopupGrid);
    }
}