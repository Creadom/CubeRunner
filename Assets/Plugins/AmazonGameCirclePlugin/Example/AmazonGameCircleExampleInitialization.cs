/**
 * © 2012-2013 Amazon Digital Services, Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"). You may not use this file except in compliance with the License. A copy
 * of the License is located at
 *
 * http://aws.amazon.com/apache2.0/
 *
 * or in the "license" file accompanying this file. This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
 */
using UnityEngine;
using System.Collections;

/// <summary>
/// Amazon GameCircle sample menu for initializing GameCircle.
/// </summary>
public class AmazonGameCircleExampleInitialization : AmazonGameCircleExampleBase {
 
    // The current status of GameCircle's initialization.
    public enum EInitializationStatus {
        Uninitialized,
        InitializationRequested,
        Ready,
        Unavailable,
    }
    
    #region properties    
    /// <summary>
    /// Gets the initialization status of GameCircle.
    /// </summary>
    /// <value>
    /// The initialization status of GameCircle.
    /// </value>
    public EInitializationStatus InitializationStatus {
        get { return initializationStatus; }   
    }
    private EInitializationStatus initializationStatus = EInitializationStatus.Uninitialized;
    #endregion
    
    #region local variables
    
    // What time GameCircle was requested to initialize.
    // Used to show a timer to show the menu is active, and is just waiting on a callback.
    System.DateTime initRequestTime;
    
    // GameCircle features to be used
    bool usesLeaderboards = true;
    bool usesAchievements = true;
    bool usesWhispersync = true;
    
    // Location of GameCircle popups
    GameCirclePopupLocation toastLocation = GameCirclePopupLocation.BOTTOM_CENTER;
    // The toast locations enum converted to strings.
    string [] toastLocations = null;
    // Toggles popups enabled / disabled.
    bool enablePopups = true;
    
    private string gameCircleInitializationStatusLabel = null;
    #endregion
    
    #region constructor
    public AmazonGameCircleExampleInitialization() {
        toastLocations = System.Enum.GetNames(typeof(GameCirclePopupLocation));   
    }
    #endregion
    
    /// <summary>
    /// Strings used by the UI.
    /// </summary>
    #region Local const strings
    // The name of the GameCircle plugin.
    private const string pluginName = "Amazon GameCircle";
    // The text to display in the button used to initialize the plugin.
    private readonly string pluginInitializationButton = string.Format("Initialize {0}",pluginName);
    
    // The title of this menu
    private const string initializationmenuTitle = "Initialization";
    
    // Labels for selecting what GameCircle features will be used
    private const string usesLeaderboardsLabel = "Use Leaderboards";
    private const string usesAchievementsLabel = "Use Achievements";
    private const string usesWhispersyncLabel = "Use Whispersync";
    
    // Label placed about the toast / popup location selection
    private const string toastLocationLabel = "Popup Location";
    // Labels for displaying the popup status.
    private const string popupsDisabledLabel = "Popups Disabled";
    private const string popupsEnabledLabel = "Popups Enabled";
    
    // Labels for when the GameCircle initialization callbacks occur.
    private const string pluginFailedToInitializeLabel = "Failed to initialize: {0}";
    private readonly string pluginInitializedLabel = string.Format("{0} is ready for use.",pluginName);
    
    // a label to display how much time is spent waiting for GameCircle to initialize.
    private const string loadingTimeLabel = "{0,5:N1} seconds";
    #endregion
    
    #region base class implementation
    /// <summary>
    /// The title of the menu.
    /// </summary>
    /// <returns>
    /// The title of the menu.
    /// </returns>
    public override string MenuTitle() {
        return initializationmenuTitle;
    }
    /// <summary>
    /// Draws the menu. Note that this must be called from an OnGUI function.
    /// </summary>
    public override void DrawMenu() {        
        
        switch(InitializationStatus) {
        case EInitializationStatus.Uninitialized:
            DisplayInitGameCircleMenu();
            break;
        case EInitializationStatus.InitializationRequested:
            AmazonGUIHelpers.BoxedCenteredLabel(pluginName);
            DisplayLoadingGameCircleMenu();
            break;
        case EInitializationStatus.Unavailable:
            DisplayGameCircleUnavailableMenu();
            break;
        default:
            break;
        }
        
    }
    #endregion
        
    #region Menu Functions
    /// <summary>
    /// Displays the menu for initializing GameCircle
    /// </summary>
    private void DisplayInitGameCircleMenu() {
        if(GUILayout.Button(string.Format(pluginInitializationButton,pluginName))) {
            InitializeGameCircle();   
        }
        
        GUILayout.BeginHorizontal();
        GUILayout.Label(GUIContent.none);
        
        GUILayout.BeginVertical(GUI.skin.box);
        
        GUILayout.Label(GUIContent.none);
        usesLeaderboards = GUILayout.Toggle(usesLeaderboards,usesLeaderboardsLabel);
        GUILayout.Label(GUIContent.none);
        usesAchievements = GUILayout.Toggle(usesAchievements,usesAchievementsLabel);
        GUILayout.Label(GUIContent.none);
        usesWhispersync = GUILayout.Toggle(usesWhispersync,usesWhispersyncLabel);
        
        AmazonGUIHelpers.AnchoredLabel(toastLocationLabel,TextAnchor.LowerCenter);
        if(null != toastLocations) {
            toastLocation = (GameCirclePopupLocation) GUILayout.SelectionGrid((int)toastLocation,toastLocations, 3);
        }
        
        GUILayout.Label(GUIContent.none);
        
        if(GUILayout.Button(enablePopups ? popupsEnabledLabel : popupsDisabledLabel)) {
            enablePopups = !enablePopups;
        }
        
        GUILayout.EndVertical();
        
        
        GUILayout.Label(GUIContent.none);
        
        GUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// Displays the status of the GameCircle plugin loading.
    /// </summary>
    private void DisplayLoadingGameCircleMenu() {
        if(!string.IsNullOrEmpty(gameCircleInitializationStatusLabel)) {
            AmazonGUIHelpers.CenteredLabel(gameCircleInitializationStatusLabel);
        }
        AmazonGUIHelpers.CenteredLabel(string.Format(loadingTimeLabel,(System.DateTime.Now - initRequestTime).TotalSeconds));
    }
    
    /// <summary>
    /// Displays the menu for when GameCircle is unavailable.
    /// </summary>
    private void DisplayGameCircleUnavailableMenu() {
        if(!string.IsNullOrEmpty(gameCircleInitializationStatusLabel)) {
            AmazonGUIHelpers.CenteredLabel(gameCircleInitializationStatusLabel);
        }
    }
    #endregion
    
    #region GameCircle plugin functions
    /// <summary>
    /// Initializes the GameCircle plugin.
    /// </summary>
    void InitializeGameCircle() {
        // Step the initialization progress forward.
        initializationStatus = EInitializationStatus.InitializationRequested;
        
        // Subscribe to the initialization events so the menu knows when GameCircle is ready (or errors out)
        SubscribeToGameCircleInitializationEvents();
        
        // Start a timer to help show this is an asynchronous event.
        initRequestTime = System.DateTime.Now;
        
        // Begin GameCircle initialization.
        AGSClient.Init(usesLeaderboards, usesAchievements, usesWhispersync);
    }
    #endregion
    
    #region Callback helper functions
    /// <summary>
    /// Subscribes to GameCircle initialization events.
    /// </summary>
    private void SubscribeToGameCircleInitializationEvents() {   
        AGSClient.ServiceReadyEvent += ServiceReadyHandler;
        AGSClient.ServiceNotReadyEvent += ServiceNotReadyHandler;
    }
    
    /// <summary>
    /// Unsubscribes from GameCircle initialization events.
    /// </summary>
    private void UnsubscribeFromGameCircleInitializationEvents() {   
        AGSClient.ServiceReadyEvent -= ServiceReadyHandler;
        AGSClient.ServiceNotReadyEvent -= ServiceNotReadyHandler;
    }
    #endregion
    
    #region GameCircle plugin callbacks
    /// <summary>
    /// Callback if GameCircle fails to initialize.
    /// </summary>
    /// <param name='error'>
    /// Error message.
    /// </param>
    private void ServiceNotReadyHandler(string error) {
        initializationStatus = EInitializationStatus.Unavailable;
        gameCircleInitializationStatusLabel = string.Format(pluginFailedToInitializeLabel,error);
        // Once the callback is received, these events do not need to be subscribed to.
        UnsubscribeFromGameCircleInitializationEvents();
    }   
      
    /// <summary>
    /// Callback when GameCircle is initialized and ready to use.
    /// </summary>
    private void ServiceReadyHandler() {
        initializationStatus = EInitializationStatus.Ready;
        gameCircleInitializationStatusLabel = pluginInitializedLabel;
        // Once the callback is received, these events do not need to be subscribed to.
        UnsubscribeFromGameCircleInitializationEvents();
        // Tell the GameCircle plugin the popup information set here.
        // Calling this after GameCircle initialization is safest.
        AGSClient.SetPopUpEnabled(enablePopups);
        AGSClient.SetPopUpLocation(toastLocation);
    }
    #endregion
}
