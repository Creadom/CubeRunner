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
/// GameCircle manager.
/// </summary>
/// <remarks>
/// Helper script for managing native to unity messages
/// </remarks>

public class GameCircleManager : MonoBehaviour
{
    
    void Awake()
    {
        // Set the GameObject name to the class name for easy access from native code
        gameObject.name = this.GetType().ToString();
        DontDestroyOnLoad( this );
    }

    public void serviceReady( string empty )
    {        
        AGSClient.Log ("GameCircleManager - serviceReady");
        
        AGSClient.
            ServiceReady(empty);
    }
    
    public void serviceNotReady( string param )
    {
        AGSClient.Log ("GameCircleManager - serviceNotReady");

        AGSClient.
            ServiceNotReady( param );
    }
    
    public void playerReceived( string json )
    {
        AGSClient.Log ("GameCircleManager - playerReceived");
        AGSPlayerClient.
            PlayerReceived( json );
    }
    
    public void playerFailed( string json )
    {
        AGSClient.Log ("GameCircleManager - playerFailed");
        AGSPlayerClient.
            PlayerFailed( json );
    }

    public void localPlayerFriendRequestComplete(string json) {
        AGSClient.Log ("GameCircleManager - localPlayerFriendRequestComplete");
        AGSPlayerClient.LocalPlayerFriendsComplete (json);
    }

    public void batchFriendsRequestComplete (string json) {
        AGSClient.Log ("GameCircleManager - batchFriendsRequestComplete");
        AGSPlayerClient.BatchFriendsRequestComplete (json);
    }

    public void onSignedInStateChange (string isSignedIn) {
        AGSClient.Log ("GameCircleManager - onSignedInStateChange");
        AGSPlayerClient.OnSignedInStateChanged (Boolean.Parse (isSignedIn));
    }

    public void submitScoreFailed( string json )
    {
        AGSClient.Log ("GameCircleManager - submitScoreFailed");

        AGSLeaderboardsClient.
            SubmitScoreFailed( json );
    }

    public void submitScoreSucceeded( string json )
    {
        AGSClient.Log ("GameCircleManager - submitScoreSucceeded");
        AGSLeaderboardsClient.
            SubmitScoreSucceeded( json );
    }

    public void requestLeaderboardsFailed( string json )
    {
        AGSClient.Log ("GameCircleManager - requestLeaderboardsFailed");
        AGSLeaderboardsClient.
            RequestLeaderboardsFailed( json );
    }

    public void requestLeaderboardsSucceeded( string json )
    {    
        AGSClient.Log ("GameCircleManager - requestLeaderboardsSucceeded");
        AGSLeaderboardsClient.
            RequestLeaderboardsSucceeded(json);
    }

    public void requestLocalPlayerScoreFailed( string json )
    {
        AGSClient.Log ("GameCircleManager - requestLocalPlayerScoreFailed");
        AGSLeaderboardsClient.
            RequestLocalPlayerScoreFailed( json );
    }

    public void requestLocalPlayerScoreSucceeded( string json )
    {
        AGSClient.Log ("GameCircleManager - requestLocalPlayerScoreSucceeded");
        AGSLeaderboardsClient.
                RequestLocalPlayerScoreSucceeded(json);
    }

    public void requestPlayerScoreCompleted( string json )
    {
        AGSClient.Log ("GameCircleManager - requestPlayerScoreCompleted");
        AGSLeaderboardsClient.RequestScoreForPlayerComplete (json);
    }


    public void requestScoresSucceeded( string json )
    {
        AGSClient.Log ("GameCircleManager - requestScoresSucceeded:");
        AGSLeaderboardsClient.RequestScoresSucceeded(json);
    }

    public void requestScoresFailed( string json )
    {
        AGSClient.Log ("GameCircleManager - requestScoresFailed");
        AGSLeaderboardsClient.RequestScoresFailed(json);
    }
    
    public void requestPercentileRanksSucceeded(string json)
    {
        AGSClient.Log ("GameCircleManager - requestPercentileRanksSucceeded");
        AGSLeaderboardsClient.RequestPercentileRanksSucceeded(json);
    }
    
    public void requestPercentileRanksFailed(string json)
    {
        AGSClient.Log ("GameCircleManager - requestPercentileRanksFailed");
        AGSLeaderboardsClient.RequestPercentileRanksFailed(json);
    }

    public void requestPercentileRanksForPlayerSucceeded(string json)
    {
        AGSClient.Log ("GameCircleManager - requestPercentileRanksForPlayerSucceeded");
        AGSLeaderboardsClient.RequestPercentileRanksForPlayerComplete (json);
    }

    public void updateAchievementSucceeded( string json )
    {
        AGSClient.Log ("GameCircleManager - updateAchievementSucceeded");
        AGSAchievementsClient.UpdateAchievementSucceeded( json );
    }
    
    public void updateAchievementFailed( string json )
    {
        AGSClient.Log ("GameCircleManager - updateAchievementsFailed");
        AGSAchievementsClient.
            UpdateAchievementFailed( json );
    }
    
    public void requestAchievementsSucceeded( string json )
    {    
        AGSClient.Log ("GameCircleManager - requestAchievementsSucceeded");

        AGSAchievementsClient.
            RequestAchievementsSucceeded( json );
    }
    
    public void requestAchievementsFailed( string json )
    {
        AGSClient.Log ("GameCircleManager -  requestAchievementsFailed");
        AGSAchievementsClient.
            RequestAchievementsFailed( json );
    }

    public void requestAchievementsForPlayerCompleted(string json)
    {
        AGSClient.Log ("GameCircleManager -  requestAchievementsForPlayerCompleted");
        AGSAchievementsClient.RequestAchievementsForPlayerComplete (json);
    }


    public void onNewCloudData( string empty ){
        AGSWhispersyncClient.OnNewCloudData();    
    }

    public void onDataUploadedToCloud (string empty){
        AGSWhispersyncClient.OnDataUploadedToCloud ();
    }

    public void onThrottled (string empty){
        AGSWhispersyncClient.OnThrottled ();
    }

    public void onDiskWriteComplete (string empty){
        AGSWhispersyncClient.OnDiskWriteComplete ();
    }
    
    public void onFirstSynchronize (string empty){
        AGSWhispersyncClient.OnFirstSynchronize ();
    }

    public void onAlreadySynchronized (string empty){
        AGSWhispersyncClient.OnAlreadySynchronized ();
    }

    public void onSyncFailed(string failReason){
        AGSWhispersyncClient.OnSyncFailed (failReason);
    }

    public void OnApplicationFocus(Boolean focusStatus){
        
        if (!AGSClient.ReinitializeOnFocus){
            return;
        }
        
        if(focusStatus){
            AGSClient.Init();
        } else {
            AGSClient.release();
        }
    }
    
    public void OnAppplicationQuit(){
        AGSClient.Log ("GameCircleManager - OnApplicationQuit");
        AGSClient.Shutdown();
    }
    
}

