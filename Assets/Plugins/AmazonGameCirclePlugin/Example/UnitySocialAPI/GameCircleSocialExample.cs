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
using UnityEngine.SocialPlatforms;

/// <summary>
/// GameCircle example implementation of the GameCircleSocial plugin.
/// </summary>
public class GameCircleSocialExample : MonoBehaviour {    
    
    /// <summary>
    /// Enum to help track the status of async operations
    /// </summary>
    public enum AsyncOperationStatus {
        Inactive,
        Waiting,
        Failed,
        Success,
    }
    #region UI variables
    // scroll position of the example UI
    Vector2 scroll = Vector2.zero;
    // Initialization status of the UI
    bool uiInitialized = false;
    // A local GUI skin, modified to be more touch friendly
    GUISkin localGuiSkin;
    // The skin the GUI had when OnGUI began
    GUISkin originalGuiSkin;
    #endregion
    
    #region local variables
    // track the initialization status of the Social API.
    AsyncOperationStatus socialInitialization = AsyncOperationStatus.Inactive;
    // This submenu handles achievement functionality.
    GameCircleSocialAchievementsExample achievementsExample = new GameCircleSocialAchievementsExample();
    // This submenu handles leaderboard functionality.
    GameCircleSocialLeaderboardsExample leaderboardsExample = new GameCircleSocialLeaderboardsExample();
    #endregion
    
    #region local strings
    // The label for the button that begins initialization.
    private const string initializeSocialButtonLabel = "Initialize GameCircle Social API";
    // The label for the text displayed while waiting on initialization.
    private const string waitingForSocialInitialization = "Initializing...";
    // The label for the text displayed when the social API fails to initialize.
    private const string initializationFailedLabel = "Failed to initialize";
    // The label for the text displayed when the social API fails to initialize.
    private const string initializationSuccessfulLabel = "GameCircle Initialized";
    // The label for waiting on async operations
    private const string waitingLabel = "Waiting on async operation...";
    #endregion
    
    #region Unity overrides
    /// <summary>
    /// Unity MonoBehaviour Start override, for initialization.
    /// </summary>
	void Start () {
        // Tell the Social API to use GameCircle on iOS and Android.
#if UNITY_IOS || UNITY_ANDROID
        Social.Active = GameCircleSocial.Instance;	
#endif
	}
	
    /// <summary>
    /// Unity MonoBehaviour OnGUI override, for displaying menus.
    /// </summary>
	void OnGUI() {
        // Some initialization behaviour can only be called from within the OnGUI function.
        // initialize UI early returns if it is already initialized
        InitializeUI();
        ApplyLocalUISkin();
        AmazonGUIHelpers.BeginMenuLayout();
        
        // Wrapping all of the menu in a scroll view allows the individual menu systems to not need to handle being off screen.
        scroll = GUILayout.BeginScrollView(scroll);
        
        // Track the state of the Social API to display the correct menu.
        switch(socialInitialization) {
        case AsyncOperationStatus.Inactive:
            DrawInitializationMenu();
            break;
        case AsyncOperationStatus.Waiting:
            AmazonGUIHelpers.CenteredLabel(waitingForSocialInitialization);
            break;
        case AsyncOperationStatus.Failed:
            AmazonGUIHelpers.CenteredLabel(initializationFailedLabel);
            break;
        case AsyncOperationStatus.Success:
            DrawSocialMenu();
            break;
        }
        GUILayout.EndScrollView();
        
        AmazonGUIHelpers.EndMenuLayout();
        
        RevertLocalUISkin();
    }
    #endregion
    
    #region menu functions
    /// <summary>
    /// Initializes the UI for the GameCircle example menu. If already initialized, bails out.
    /// This function needs to be called from OnGUI to access GUI features.
    /// </summary>
    void InitializeUI() {
        if(uiInitialized) {
            return;
        }
        uiInitialized = true;
        localGuiSkin = GUI.skin;
        originalGuiSkin = GUI.skin;
        
        AmazonGUIHelpers.SetGUISkinTouchFriendly(localGuiSkin);
        
    }
        
    /// <summary>
    /// Applies the local user interface skin.
    /// Working in a local skin allows the example
    /// code to make UI changes without effecting
    /// other UI elements in Unity.
    /// </summary>
    void ApplyLocalUISkin() {
        GUI.skin = localGuiSkin;
    }
    
    /// <summary>
    /// Reverts the local user interface skin.
    /// Working in a local skin allows the example
    /// code to make UI changes without effecting
    /// other UI elements in Unity.
    /// </summary>
    void RevertLocalUISkin() {
        GUI.skin = originalGuiSkin;   
    }
    
    /// <summary>
    /// Draws the initialization menu.
    /// </summary>
    void DrawInitializationMenu() {
        if(GUILayout.Button(initializeSocialButtonLabel)) {
            if(null != Social.localUser) {
                socialInitialization = AsyncOperationStatus.Waiting;
                Social.localUser.Authenticate(AuthenticateCallback);
            }
        }
	}
    
    /// <summary>
    /// Draws the Social menu.
    /// </summary>
    void DrawSocialMenu() {
        // Display a message that the Social API is available.
        AmazonGUIHelpers.CenteredLabel(initializationSuccessfulLabel);
        
        // Put a box around each submenu to visually separate them from each other.
        GUILayout.BeginVertical(GUI.skin.box);
        achievementsExample.DrawAchievementsMenu();
        GUILayout.EndVertical();
        
        GUILayout.BeginVertical(GUI.skin.box);
        leaderboardsExample.DrawLeaderboardsMenu();
        GUILayout.EndVertical();
        
    }
    #endregion
    
    #region callbacks
    /// <summary>
    /// Callback for the authentication status of the Social API.
    /// </summary>
    /// <param name='success'>
    /// Success.
    /// </param>
    void AuthenticateCallback(bool success) {
        // Change the state of this menu to track the Social API status.
        if(success) {
            socialInitialization = AsyncOperationStatus.Success;
        }
        else {
            socialInitialization = AsyncOperationStatus.Failed;
        }
    }
    #endregion
}
