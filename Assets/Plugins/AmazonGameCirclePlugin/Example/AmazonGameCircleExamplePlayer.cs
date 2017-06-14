/**
 * © 2012-2014 Amazon Digital Services, Inc. All rights reserved.
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
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Amazon GameCircle example implementation of player retrieval.
/// </summary>
public class AmazonGameCircleExamplePlayer : AmazonGameCircleExampleBase {

    #region Local variables
    // These strings are updated once
    // player begins retrieval.
    private string playerStatus = null;
    private string playerStatusMessage = null;
    // the player information.
    private AGSPlayer player = null;
    private System.DateTime? lastSignInStateChangeEvent = null;
    private Boolean haveGotStateChangeEvent = false;
    private Boolean signedInStateChange;
    private List<string> friendIds;
    private string friendsRequestError = "";
    private string friendsStatus;
    private List<AGSPlayer> friends;
    #endregion

    #region Local const strings
    // The title of this menu
    private const string playerMenuTitle = "Player";
    // UI labels for player retrieval callbacks.
    private const string playerReceivedLabel = "Retrieved local player data";
    private const string playerFailedLabel = "Failed to retrieve local player data";
    // label for the button that begins player retrieval
    private const string playerRetrieveButtonLabel = "Retrieve local player data";
    // label for displaying player information
    private const string playerLabel = "ID: {0} Alias: {1}\nAvatarUrl: {2}";
    // label for displaying that player retrieval has begun.
    private const string playerRetrievingLabel = "Retrieving local player data...";
    // label for displaying whether the player is signed in.
    private const string isSignedInLabel = "Signed In: {0}";
    private const string getFriendsButtonLabel = "Get Friend ID's";
    private const string friendsLabel = "Friends ID's:";
    private const string friendsRetrievingLabel = "Retrieving friends...";
    private const string friendsRequestFailedLabel = "Friends request failed: {0}";
    // label for showing how long since the player signed in.
    private const string signedInEventLabel = "Player signed in {0,5:N1} seconds ago.";
    // label for showing how long since the player signed out.
    private const string signedOutEventLabel = "Player signed out {0,5:N1} seconds ago.";
    // displaying "null" instead of an empty string looks nicer in the UI
    private const string nullAsString = "null";

    #endregion

    #region Instance management

    private static AmazonGameCircleExamplePlayer instance;

    private AmazonGameCircleExamplePlayer() {
        AGSPlayerClient.RequestLocalPlayerCompleted += OnPlayerRequestCompleted;
        AGSPlayerClient.RequestFriendIdsCompleted += OnGetFriendIdsCompleted;
        AGSPlayerClient.OnSignedInStateChangedEvent += OnSignedInStateChanged;
        AGSPlayerClient.RequestBatchFriendsCompleted += OnBatchFriendsRequestCompleted;
    }

    public static AmazonGameCircleExamplePlayer Instance ( ) {
        if (instance == null) {
            instance = new AmazonGameCircleExamplePlayer ( );
        }
        return instance;
    }

    #endregion

    #region base class implementation

    /// <summary>
    /// The title of the menu.
    /// </summary>
    /// <returns>
    /// The title of the menu.
    /// </returns>
    public override string MenuTitle() {
        return playerMenuTitle;
    }
    /// <summary>
    /// Draws the GameCircle Player Menu. Note that this must be called from an OnGUI function.
    /// </summary>
    public override void DrawMenu() {

        AmazonGUIHelpers.CenteredLabel (string.Format(isSignedInLabel, AGSPlayerClient.IsSignedIn() ? "true" : "false" ));

        // Once the Status string is not null, player retrieval has begun.
        // This button begins the player retrieval process.
        if(GUILayout.Button(playerRetrieveButtonLabel)) {
            AGSPlayerClient.RequestLocalPlayer();
            playerStatus = playerRetrievingLabel;
        }

        if (!string.IsNullOrEmpty(playerStatus)) {
            AmazonGUIHelpers.CenteredLabel(playerStatus);
            // If there is a status / error message, display it.
            if(!string.IsNullOrEmpty(playerStatusMessage)) {
                AmazonGUIHelpers.CenteredLabel(playerStatusMessage);
            }
            // player has been received, display it.
            if(null != player) {
                AmazonGUIHelpers.CenteredLabel(player.ToString());
            }
        }

        if (GUILayout.Button (getFriendsButtonLabel)) {
            AGSPlayerClient.RequestFriendIds();
        }

        if (!string.IsNullOrEmpty (friendsRequestError)) {
            AmazonGUIHelpers.CenteredLabel(string.Format(friendsRequestFailedLabel, friendsRequestError));
        }

        if (friendIds != null) {
            AmazonGUIHelpers.CenteredLabel(friendsLabel);
            foreach (string friendId in friendIds) {
                AmazonGUIHelpers.CenteredLabel(friendId);
            }

            if (GUILayout.Button("Request Friends")) {
                friendsStatus = "Requesting friends...";
                AGSPlayerClient.RequestBatchFriends(friendIds);
            }

        }

        if (!string.IsNullOrEmpty (friendsStatus)) {
            AmazonGUIHelpers.CenteredLabel(friendsStatus);
        }


        if (friends != null) {
            AmazonGUIHelpers.CenteredLabel("Friends:");
            foreach (AGSPlayer friend in friends ) {
                AmazonGUIHelpers.CenteredLabel(friend.ToString());
            }
        }

        // If a signed in state change event has happened, display when it happened.
        if (haveGotStateChangeEvent && lastSignInStateChangeEvent != null) {
            double timeElapsed = (System.DateTime.Now - lastSignInStateChangeEvent.Value).TotalSeconds;
            if (signedInStateChange) {
                AmazonGUIHelpers.CenteredLabel(string.Format(signedInEventLabel, timeElapsed));
            } else {
                AmazonGUIHelpers.CenteredLabel(string.Format(signedOutEventLabel, timeElapsed));
            }
        }

    }
    #endregion

    #region Callbacks

    private void OnPlayerRequestCompleted(AGSRequestPlayerResponse response) {
        if (response.IsError ()) {
            playerStatus = playerFailedLabel;
            playerStatusMessage = response.error;
            this.player = null;
        } else {
            playerStatus = playerReceivedLabel;
            playerStatusMessage = null;
            this.player = response.player;
        }
    }

    private void OnGetFriendIdsCompleted(AGSRequestFriendIdsResponse response) {
        if (response.IsError ()) {
            friendIds = null;
            friendsRequestError = response.error;
        } else {
            friendsRequestError = "";
            friendIds = response.friendIds;
        }

    }

    private void OnBatchFriendsRequestCompleted (AGSRequestBatchFriendsResponse response) {
        if (response.IsError ()) {
            friendsStatus = string.Format("Error requesting friends: {0}", response.error);
        } else {
            friendsStatus = "";
            friends = response.friends;
        }
    }

    /// <summary>
    /// Raises the signed in state changed event.
    /// </summary>
    /// <param name="isSignedIn">If set to <c>true</c>, the local player is signed in.</param>
    private void OnSignedInStateChanged(Boolean isSignedIn) {
        this.haveGotStateChangeEvent = true;
        this.signedInStateChange = isSignedIn;
        this.lastSignInStateChangeEvent = System.DateTime.Now;
    }

    #endregion

}
