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
using System.Collections.Generic;

/// <summary>
/// Amazon GameCircle implementation of example Leaderboard functionality.
/// </summary>
public class AmazonGameCircleExampleLeaderboards : AmazonGameCircleExampleBase {

    #region Leaderboard Variables
    // Key: Leaderboard ID, value: leaderboard submission status
    Dictionary<string,string> leaderboardsSubmissionStatus = new Dictionary<string, string>();
    // Key: Leaderboard ID, value: leaderboard submission message (any errors that occured)
    Dictionary<string,string> leaderboardsSubmissionStatusMessage = new Dictionary<string, string>();
    // Key: Leaderboard ID, value: leaderboard local score request status
    Dictionary<string,string> leaderboardsLocalScoreStatus = new Dictionary<string, string>();
    // Key: Leaderboard ID, value: leaderboard local score request message (any errors that occured)
    Dictionary<string,string> leaderboardsLocalScoreStatusMessage = new Dictionary<string, string>();
    // Key: Leaderboard ID, value: leaderboard local score request status
    Dictionary<string, List<string>> leaderboardsScoresStatus = new Dictionary<string, List<string>>();
    // Key: Leaderboard ID, value: leaderboard local score request message (any errors that occured)
    Dictionary<string,string> leaderboardsScoresStatusMessage = new Dictionary<string, string>();
    // Key: Leaderboard ID, value: leaderboard local score request status
    Dictionary<string, List<string>> leaderboardsPercentilesStatus = new Dictionary<string, List<string>>();
    // Key: Leaderboard ID, value: leaderboard local score request message (any errors that occured)
    Dictionary<string,string> leaderboardsPercentilesStatusMessage = new Dictionary<string, string>();
    // Key: Leaderboard ID, value: whether to show the foldout for player scores.
    Dictionary<string, bool> showPlayerScoreRequestButtons = new Dictionary<string, bool>();
    // Key: Leaderboard ID, value: (Dictionary where key: Player ID, value: result of requesting their score)
    Dictionary<string, Dictionary<string, string>> scoreForPlayer = new Dictionary<string, Dictionary<string, string>>();
    // Key: Leaderboard ID, value: whether to show the foldout for the player percentiles request.
    Dictionary<string, bool> showPlayerPercentileRequestButtons = new Dictionary<string, bool>();
    // Key: Leaderboard ID, value:  (Dictionary where key: Player ID, value: Percentiles info)
    Dictionary<string, Dictionary<string, List<string>>> percentilesForPlayer = new Dictionary<string, Dictionary<string, List<string>>>();

    // Key: Leaderboard ID, value: if the leaderboard is folded out.
    Dictionary<string,bool> leaderboardsFoldout = new Dictionary<string, bool>();
    // List of friends (used for getting player score or leaderboard percentiles).
    List<string> friendIds = null;
    // The status of getting friend info.
    string friendStatus = null;
    // This menu uses a shared value for all leaderboard score submissions
    long leaderboardScoreValue = 0;
    // Status messages for showing the status of leaderboard retrieval
    string requestLeaderboardsStatus = null;
    string requestLeaderboardsStatusMessage = null;
    // The list of retrieved leaderboards
    List<AGSLeaderboard> leaderboardList = null;
    // Are the leaderboards ready for this? This is used instead of
    // checking the leaderboard list for null, because if there are no
    // leaderboards available, the success callback will return null.
    bool leaderboardsReady = false;
    // The time the leaderboard list request began. This helps
    // show that this is an asynchronous operation.
    System.DateTime leaderboardsRequestTime;
    // The scope of leaderboard score requests.
    LeaderboardScope leaderboardScoreScope = LeaderboardScope.GlobalAllTime;
     // an invalid leaderboard used to test that GameCircle fails gracefully.
    AGSLeaderboard invalidLeaderboard;
    #endregion

    #region UI variables
    // define the regex once here to save on re-creating the regex
    // This regex is intended to add a newline after every second comma in a string.
    // See function addNewlineAfterSecondComma for full implementation of this regex.
    readonly System.Text.RegularExpressions.Regex addNewlineEverySecondCommaRegex = new System.Text.RegularExpressions.Regex(@"(,([^,]+),)");
    // The group number to use with the regex for keeping the text between commas.
    const int betweenCommaRegexGroup = 2;
    #endregion

    #region Local const strings
    // The title of this menu
    private const string leaderboardsMenuTitle = "Leaderboards";
    // The label for the button that opens the leaderboards overal.
    private const string DisplayLeaderboardOverlayButtonLabel = "Leaderboards Overlay";
    // The label for the button that requests the list of leaderboards.
    private const string requestLeaderboardsButtonLabel = "Request Leaderboards";
    // The status message to display after the leaderboard list request has begun.
    private const string requestingLeaderboardsLabel = "Requesting Leaderboards...";
    // The status message to display if an error occurs retrieving the leaderboard list.
    private const string requestLeaderboardsFailedLabel = "Request Leaderboards failed with error:";
    // The status message to display if the leaderboard list request succeeded.
    private const string requestLeaderboardsSucceededLabel = "Available Leaderboards";
    // The message to display if there are no leaderboards in the list.
    private const string noLeaderboardsAvailableLabel = "No Leaderboards Available";
    // The label used to display leaderboard IDs.
    private const string leaderboardIDLabel = "Leaderboard \"{0}\"";
    // The label used for displaying the time elapsed since the leaderboard list was requested.
    private const string leaderboardRequestTimeLabel = "{0,5:N1} seconds";
    // The label to display the score for leaderboard score submission.
    private const string leaderboardScoreDisplayLabel = "{0} score units";
    // The label for the button used to submit a new leaderboard score.
    private const string submitLeaderboardButtonLabel = "Submit Score";
    // The status message to display if an error occurs submitting a score.
    private const string leaderboardFailed = "Leaderboard \"{0}\" failed with error:";
    // The status message to display if the leaderboard score submission succeeds.
    private const string leaderboardSucceeded = "Score uploaded to \"{0}\" successfully.";
    // The label for the button used to request the local player's score on a leaderboard.
    private const string requestLeaderboardScoreButtonLabel = "Request Local Player Score";
    // The label used to display the local player's score on the leaderboard.
    private const string leaderboardRankScoreLabel = "Rank {0} with score of {1}.";
    // The label to use if the local player score request failed.
    private const string leaderboardScoreFailed = "\"{0}\" score request failed with error:";
    // The label to request the top scores for the leaderboard
    private const string requestTopScoresButtonLabel = "Request Top Scores";
    // The label to use if the percentiles request failed.
    private const string topScoresFailed = "\"{0}\" top scores request failed with error:";
    // The label to use to describe a top score item.
    private const string topScoreItemLabel = "Player: {0}\nScore: {1:D}, FormattedScore:{2}, Rank: {3:D}";
    // The label used to request the percentile ranks for the leaderboard.
    private const string requestLeaderboardPercentilesButtonLabel = "Request Leaderboard Percentiles";
    // The label to use if the percentiles request failed.
    private const string percentilesFailed = "\"{0}\" percentiles request failed with error:";
    // The label to use to describe a percentile rank.
    private const string percentileRankLabel = "Player: {0}\nPercentile: {1:D}, Score: {2:D}";
    // The label to use to indicate the local user's index in a percentile ranks response.
    private const string localPlayerIndexLabel = "Local user has an index of {0:D}.";
    #endregion

    #region local const values (non-string)
    // The minimum and maximum values used for the leaderboard score slider.
    private const int leaderboardMinValue = -10000;
    private const int leaderboardMaxValue = 10000;
    #endregion

    #region Instance management

    // Singleton instance.
    private static AmazonGameCircleExampleLeaderboards instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonGameCircleExampleLeaderboards"/> class.
    /// </summary>
    private AmazonGameCircleExampleLeaderboards() {

        AGSLeaderboardsClient.RequestLeaderboardsCompleted += OnRequestLeaderboardsCompleted;
        AGSLeaderboardsClient.SubmitScoreCompleted += OnSubmitScoreCompleted;
        AGSLeaderboardsClient.RequestLocalPlayerScoreCompleted += OnRequestLocalPlayerScoreCompleted;
        AGSLeaderboardsClient.RequestScoresCompleted += OnRequestScoresCompleted;
        AGSLeaderboardsClient.RequestPercentileRanksCompleted += OnRequestPercentileRanksCompleted;
        AGSLeaderboardsClient.RequestPercentileRanksCompleted += OnRequestPercentileRanksCompleted;
        AGSLeaderboardsClient.RequestScoreForPlayerCompleted += OnRequestScoreForPlayerCompleted;
        AGSLeaderboardsClient.RequestPercentileRanksForPlayerCompleted += OnRequestPercentilesForPlayerCompleted;
        AGSPlayerClient.RequestFriendIdsCompleted += OnRequestFriendsCompleted;

        // This is used to test the GameCircle plugin's behaviour with invalid leaderboard information.
        // This leaderboard ID should not be in your game's list, to test what happens with invalid leaderboard submissions.
        invalidLeaderboard = new AGSLeaderboard();
        invalidLeaderboard.id = "Invalid Leaderboard ID";
    }

    public static AmazonGameCircleExampleLeaderboards Instance () {
        if (instance == null) {
            instance = new AmazonGameCircleExampleLeaderboards();
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
        return leaderboardsMenuTitle;
    }
    /// <summary>
    /// Draws the menu. Note that this must be called from an OnGUI function.
    /// </summary>
    public override void DrawMenu() {
        // this button will open the leaderboard overlay.
        if(GUILayout.Button(DisplayLeaderboardOverlayButtonLabel)) {
            AGSLeaderboardsClient.ShowLeaderboardsOverlay();
        }

        // If the leaderboard list has not been requested yet, display
        // a button that requests the leaderboard list.
        if(string.IsNullOrEmpty(requestLeaderboardsStatus)) {

            if(GUILayout.Button(requestLeaderboardsButtonLabel)) {
                // Start the clock, to track the progress of this async operation.
                leaderboardsRequestTime = System.DateTime.Now;
                // Request the leaderboard list from the GameCircle plugin.
                AGSLeaderboardsClient.RequestLeaderboards();
                // Set the status message to show this process has begun.
                requestLeaderboardsStatus = requestingLeaderboardsLabel;
            }

        } else {
            // once a request has been made for the list of leaderboards,
            // display the status message of that process.
            AmazonGUIHelpers.CenteredLabel(requestLeaderboardsStatus);
            if(!string.IsNullOrEmpty(requestLeaderboardsStatusMessage)) {
                AmazonGUIHelpers.CenteredLabel(requestLeaderboardsStatusMessage);
            }

            // If the leaderboards are not ready, display how long it has been since the request was put in.
            if(!leaderboardsReady) {
                AmazonGUIHelpers.CenteredLabel(string.Format(leaderboardRequestTimeLabel,(System.DateTime.Now - leaderboardsRequestTime).TotalSeconds));
            }
            else {
                // Once the leaderboard list request callback has been received,
                // display the leaderboards if available.
                if(null != leaderboardList && leaderboardList.Count > 0) {
                    foreach(AGSLeaderboard leaderboard in leaderboardList) {
                        DisplayLeaderboard(leaderboard);
                    }
                }
                // If the leaderboards are not available, display a message explaining that.
                else {
                    AmazonGUIHelpers.CenteredLabel(noLeaderboardsAvailableLabel);
                }
                // display the invalid leaderboard (used to make sure GameCircle handles invalid data gracefully)
                if(null != invalidLeaderboard) {
                    DisplayLeaderboard(invalidLeaderboard);
                }
            }
        }
    }
    #endregion


    #region UI functions
    /// <summary>
    /// Displays an individual leaderboard.
    /// </summary>
    /// <param name='leaderboard'>
    /// Leaderboard.
    /// </param>
    void DisplayLeaderboard(AGSLeaderboard leaderboard) {
        // Place a box around each leaderboard, to make it clear in the UI
        // what controls are for what leaderboard.
        GUILayout.BeginVertical(GUI.skin.box);
        // make sure this leaderboard is in the foldout dictionary.
        if(!leaderboardsFoldout.ContainsKey(leaderboard.id)) {
            leaderboardsFoldout.Add(leaderboard.id,false);
        }

        // Display a foldout for this leaderboard.
        // Foldouts keep the menu tidy.
        leaderboardsFoldout[leaderboard.id] = AmazonGUIHelpers.FoldoutWithLabel(leaderboardsFoldout[leaderboard.id],string.Format(leaderboardIDLabel,leaderboard.id));

        // If the foldout is open, display the leaderboard information.
        if(leaderboardsFoldout[leaderboard.id]) {
            // The controls for automatically word wrapping a label are not great,
            // so replacing every second comma in the string returned from the leaderboard's toString function
            // will allow for a cleaner display of each leaderboard's data
            AmazonGUIHelpers.AnchoredLabel(AddNewlineEverySecondComma(leaderboard.ToString()),TextAnchor.UpperCenter);

            // Display a centered slider, with the minimum value on the left, and maximum value on the right.
            // This lets the user select a value for the leaderboard's score.
            leaderboardScoreValue = (long)AmazonGUIHelpers.DisplayCenteredSlider(leaderboardScoreValue,leaderboardMinValue,leaderboardMaxValue,leaderboardScoreDisplayLabel);

            // If a leaderboard score submission is in progress, show its status.
            if(leaderboardsSubmissionStatus.ContainsKey(leaderboard.id) && !string.IsNullOrEmpty(leaderboardsSubmissionStatus[leaderboard.id])) {

                AmazonGUIHelpers.CenteredLabel(leaderboardsSubmissionStatus[leaderboard.id]);
                // Display any additional status message / error for this leaderboard submission.
                if(leaderboardsSubmissionStatusMessage.ContainsKey(leaderboard.id) && !string.IsNullOrEmpty(leaderboardsSubmissionStatusMessage[leaderboard.id])) {
                    AmazonGUIHelpers.CenteredLabel(leaderboardsSubmissionStatusMessage[leaderboard.id]);
                }
            }

            // This button submits an update to the leaderboard's score to the GameCircle plugin.
            if(GUILayout.Button(submitLeaderboardButtonLabel)) {
                AGSLeaderboardsClient.SubmitScore(leaderboard.id, leaderboardScoreValue);
            }

            // If a request has been made for the local user's score, display the status of that request.
            if(leaderboardsLocalScoreStatus.ContainsKey(leaderboard.id) && !string.IsNullOrEmpty(leaderboardsLocalScoreStatus[leaderboard.id])) {
                AmazonGUIHelpers.AnchoredLabel(leaderboardsLocalScoreStatus[leaderboard.id],TextAnchor.UpperCenter);
                // Display any additional information for the status message from the local score request.
                if(leaderboardsLocalScoreStatusMessage.ContainsKey(leaderboard.id) && !string.IsNullOrEmpty(leaderboardsLocalScoreStatusMessage[leaderboard.id])) {
                    AmazonGUIHelpers.AnchoredLabel(leaderboardsLocalScoreStatusMessage[leaderboard.id],TextAnchor.UpperCenter);
                }
            }

            // This button requests the local user's score.
            if(GUILayout.Button(requestLeaderboardScoreButtonLabel)) {
                AGSLeaderboardsClient.RequestLocalPlayerScore(leaderboard.id,leaderboardScoreScope);
            }

            // If a request has been made for the top scores, display the status of that request.
            if(leaderboardsScoresStatus.ContainsKey(leaderboard.id)) {
                foreach (string scoreInfo in leaderboardsScoresStatus[leaderboard.id]){
                    if (!string.IsNullOrEmpty(scoreInfo)){
                        AmazonGUIHelpers.AnchoredLabel(scoreInfo, TextAnchor.UpperCenter);
                    }
                }
                // Display any additional information for the status message from the top scores request.
                if(leaderboardsScoresStatusMessage.ContainsKey(leaderboard.id) && !string.IsNullOrEmpty(leaderboardsScoresStatusMessage[leaderboard.id])) {
                    AmazonGUIHelpers.AnchoredLabel(leaderboardsScoresStatusMessage[leaderboard.id],TextAnchor.UpperCenter);
                }
            }

            if(GUILayout.Button(requestTopScoresButtonLabel)) {
                AGSLeaderboardsClient.RequestScores(leaderboard.id, leaderboardScoreScope);
            }

            // If a request has been made for the percentile ranks, display the status of that request.
            if(leaderboardsPercentilesStatus.ContainsKey(leaderboard.id)) {
                foreach (string percentileInfo in leaderboardsPercentilesStatus[leaderboard.id]){
                    if (!string.IsNullOrEmpty(percentileInfo)){
                        AmazonGUIHelpers.AnchoredLabel(percentileInfo, TextAnchor.UpperCenter);
                    }
                }
                // Display any additional information for the status message from the percentile ranks request.
                if(leaderboardsPercentilesStatusMessage.ContainsKey(leaderboard.id) && !string.IsNullOrEmpty(leaderboardsPercentilesStatusMessage[leaderboard.id])) {
                    AmazonGUIHelpers.AnchoredLabel(leaderboardsPercentilesStatusMessage[leaderboard.id],TextAnchor.UpperCenter);
                }
            }

            if(GUILayout.Button(requestLeaderboardPercentilesButtonLabel)) {
                AGSLeaderboardsClient.RequestPercentileRanks(leaderboard.id, leaderboardScoreScope);
            }

            if (friendIds == null && GUILayout.Button("Get Friends")){
                friendStatus = "Waiting for friends...";
                AGSPlayerClient.RequestFriendIds();
            }

            if (!string.IsNullOrEmpty(friendStatus)) {
                AmazonGUIHelpers.CenteredLabel(friendStatus);
            }

            if (friendIds != null) {

                if(!showPlayerScoreRequestButtons.ContainsKey(leaderboard.id)) {
                    showPlayerScoreRequestButtons.Add(leaderboard.id,false);
                }

                GUILayout.BeginVertical(GUI.skin.box);

                showPlayerScoreRequestButtons[leaderboard.id] = AmazonGUIHelpers.FoldoutWithLabel(showPlayerScoreRequestButtons[leaderboard.id],"Request score for friend");

                if (showPlayerScoreRequestButtons[leaderboard.id]) {
                    foreach (string friend in friendIds) {
                        if (GUILayout.Button(string.Format("Score for {0}", friend))){
                            AGSLeaderboardsClient.RequestScoreForPlayer(leaderboard.id, friend, leaderboardScoreScope);
                        }

                        if (scoreForPlayer.ContainsKey(leaderboard.id) && scoreForPlayer[leaderboard.id].ContainsKey(friend)){
                            AmazonGUIHelpers.CenteredLabel(scoreForPlayer[leaderboard.id][friend]);
                        }
                    }

                }

                GUILayout.EndVertical();

                if(!showPlayerPercentileRequestButtons.ContainsKey(leaderboard.id)) {
                    showPlayerPercentileRequestButtons.Add(leaderboard.id,false);
                }

                GUILayout.BeginVertical(GUI.skin.box);
                showPlayerPercentileRequestButtons[leaderboard.id] = AmazonGUIHelpers.FoldoutWithLabel(showPlayerPercentileRequestButtons[leaderboard.id],"Request percentiles for friend");

                if (showPlayerPercentileRequestButtons[leaderboard.id]) {

                    foreach (string friendId in friendIds) {

                        if (GUILayout.Button(string.Format("Percentiles for {0}", friendId))){
                            AGSLeaderboardsClient.RequestPercentileRanksForPlayer(leaderboard.id, friendId, leaderboardScoreScope);
                        }

                        if (percentilesForPlayer.ContainsKey(leaderboard.id) && percentilesForPlayer[leaderboard.id].ContainsKey(friendId)) {
                            foreach( string row in percentilesForPlayer [leaderboard.id] [friendId] ) {
                                AmazonGUIHelpers.CenteredLabel( row );
                            }
                        }

                    }
                }
                GUILayout.EndVertical();
            }

        }

        GUILayout.EndVertical();
    }

    /// <summary>
    /// Adds a newline after every second comma in a string.
    /// It is difficult to decipher the intent of a regex from its definition,
    /// Containing the behavior of the regex within these functions makes the intent clear.
    /// </summary>
    /// <returns>
    /// a string similar to the passed in string, but with a newline after every third comma.
    /// </returns>
    /// <param name='stringToChange'>
    /// String to change.
    /// </param>
    string AddNewlineEverySecondComma(string stringToChange) {
        return addNewlineEverySecondCommaRegex.Replace(    stringToChange,
                                                        (regexMatchEvaluator) => string.Concat(",",regexMatchEvaluator.Groups[betweenCommaRegexGroup].Value,",\n"));
    }
    #endregion

    #region Callbacks

    private void OnRequestLeaderboardsCompleted( AGSRequestLeaderboardsResponse response ) {
        if (response.IsError ()) {
            // Update the status message to show the error.
            requestLeaderboardsStatus = requestLeaderboardsFailedLabel;
            requestLeaderboardsStatusMessage = response.error;
        } else {
            // Update the status message to show the request has succeeded.
            requestLeaderboardsStatus = requestLeaderboardsSucceededLabel;
            // Store the list of leaderboards.
            leaderboardList = response.leaderboards;
            // Mark the leaderboards as ready for use.
            // This bool is used instead of checking if the leaderboard list is null,
            // because the passed in leaderboard list can be null.
            // In that case, the menu should update to show that leaderboards were received, but empty.
            leaderboardsReady = true;
        }
    }

    private void OnSubmitScoreCompleted ( AGSSubmitScoreResponse response ) {
        if (response.IsError ()) {
            // Update the leaderboard submission status message to show this error.
            leaderboardsSubmissionStatus[response.leaderboardId] = string.Format(leaderboardFailed, response.leaderboardId);
            leaderboardsSubmissionStatusMessage[response.leaderboardId] = response.error;
        } else {
            // Update the leaderboard submission status message to show this has succeeded.
            leaderboardsSubmissionStatus[response.leaderboardId] = string.Format(leaderboardSucceeded, response.leaderboardId);
        }
    }

    private void OnRequestLocalPlayerScoreCompleted ( AGSRequestScoreResponse response ) {
        if (response.IsError()){
            // Update the local score request status message to show the error that occured.
            leaderboardsLocalScoreStatus[response.leaderboardId] = string.Format(leaderboardScoreFailed, response.leaderboardId);
            leaderboardsLocalScoreStatusMessage[response.leaderboardId] = response.error;
        } else {
            // Update the local score request status to show that the local score request has succeeded,
            // and update the message with the local score and rank.
            leaderboardsLocalScoreStatus[response.leaderboardId] = string.Format(leaderboardRankScoreLabel, response.rank, response.score);
        }
    }

    private void OnRequestScoresCompleted ( AGSRequestScoresResponse response ) {

        List<string> formattedResult = new List<string>();

        if (response.IsError ()) {
            formattedResult.Add(string.Format ("Error with scores request: {0}", response.error));
        } else {
            formattedResult.Add(string.Format("Leaderboard: [{0}]", response.leaderboard.ToString()));
            formattedResult.Add (string.Format("Scope: {0}", response.scope));
            foreach (AGSScore score in response.scores){
                formattedResult.Add(string.Format(topScoreItemLabel, score.player.ToString(), score.scoreValue, score.scoreString, score.rank));
            }
        }

        leaderboardsScoresStatus[response.leaderboardId] = formattedResult;

    }

    private void OnRequestPercentileRanksCompleted ( AGSRequestPercentilesResponse response ) {
        if (response.IsError ()) {
            // Update the percentiles request status message to show the error that occured.
            List<string> failureStringContainer = new List<string>();
            failureStringContainer.Add(string.Format(percentilesFailed, response.leaderboardId));
            leaderboardsPercentilesStatus[response.leaderboardId] = failureStringContainer;
            leaderboardsPercentilesStatusMessage[response.leaderboardId] = response.error;
        } else {
            // Format the response into human readable strings.
            List<string> formattedResult = new List<string>();
            foreach (AGSLeaderboardPercentile percentile in response.percentiles){
                formattedResult.Add(string.Format(percentileRankLabel, percentile.player.ToString(), percentile.percentile, percentile.score));
            }
            formattedResult.Add(string.Format(localPlayerIndexLabel, response.userIndex));
            // Update the status for the percentile response.
            leaderboardsPercentilesStatus[response.leaderboardId] = formattedResult;
        }
    }

    private void OnRequestFriendsCompleted(AGSRequestFriendIdsResponse response) {
        if (response.IsError ()) {
            friendStatus = string.Format("Error getting friends: {0}", response.error);
        } else {
            if (response.friendIds.Count > 0) {
                friendIds = response.friendIds;
                friendStatus = null;
            } else {
                friendStatus = "Local player has no friends.";
            }
        }
    }

    private void OnRequestScoreForPlayerCompleted(AGSRequestScoreForPlayerResponse response) {

        if (!scoreForPlayer.ContainsKey (response.leaderboardId)) {
            scoreForPlayer.Add(response.leaderboardId, new Dictionary<string, string>());
        }

        if (response.IsError ()) {
            scoreForPlayer [response.leaderboardId] [response.playerId] = string.Format ("Error getting score: {0}", response.error);
        } else {
            scoreForPlayer [response.leaderboardId] [response.playerId] = string.Format ("Rank: {0}, Score: {1}, Scope: {2}", response.rank, response.score, response.scope);
        }

    }

    private void OnRequestPercentilesForPlayerCompleted(AGSRequestPercentilesForPlayerResponse response) {

        if (!percentilesForPlayer.ContainsKey (response.leaderboardId)) {
            percentilesForPlayer.Add(response.leaderboardId, new Dictionary<string, List<string>>());
        }

        percentilesForPlayer [response.leaderboardId] [response.playerId] = new List<string> ();

        if (response.IsError ()) {
            percentilesForPlayer [response.leaderboardId] [response.playerId].Add(string.Format ("Error getting percentiles: {0}", response.error));
        } else {

            percentilesForPlayer [response.leaderboardId] [response.playerId].Add(string.Format("Leaderboard: {0}", response.leaderboard.ToString()));
            percentilesForPlayer [response.leaderboardId] [response.playerId].Add(string.Format ("Scope: {0}", response.scope.ToString()));

            foreach (AGSLeaderboardPercentile percentile in response.percentiles){
                percentilesForPlayer [response.leaderboardId] [response.playerId].Add(string.Format(percentileRankLabel, percentile.player.ToString(), percentile.percentile, percentile.score));
            }

            percentilesForPlayer [response.leaderboardId] [response.playerId].Add(string.Format("Requested player has an index of {0}.", response.userIndex));

        }
    }

    #endregion
}
