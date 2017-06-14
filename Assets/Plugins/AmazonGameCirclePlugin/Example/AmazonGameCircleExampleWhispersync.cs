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
using System.Collections.Generic;

/// <summary>
/// Amazon GameCircle Whispersync example implementation.
/// </summary>
public class AmazonGameCircleExampleWhispersync : AmazonGameCircleExampleBase {

    #region Whispersync Variables
    // track the last time cloud data was received
    // The ? makes it a nullable, which is an easy way
    // to check if a structure has been initialized for the first time.
    private System.DateTime? lastOnNewCloudData = null;
    private System.DateTime? lastOnDataUploadedToCloud = null;
    private System.DateTime? lastOnThrottled = null;
    private System.DateTime? lastOnDiskWriteComplete = null;
    private System.DateTime? lastOnFirstSynchronize = null;
    private System.DateTime? lastOnAlreadySynchronized = null;
    private System.DateTime? lastOnSyncFailed = null;
    private string failReason = "";

    // a foldout to hide the list of syncable numbers
    private bool syncableNumbersFoldout = false;
    // a foldout to hide the list of accumulating numbers.
    private bool accumulatingNumbersFoldout = false;
    // a foldout to hide the list of syncable number lists
    private bool syncableNumberListsFoldout = false;
    // a foldout to hide the list of hash sets
    private bool hashSetsFoldout = false;
    // a foldout to hide the developer string
    private bool developerStringFoldout = false;

    // There are a lot of test cases for Whispersync numbers:
    // Every syncable number type times every syncable behavior type.
    // This helper class breaks this down into a simple loop, instead of many functions.
    private List<AmazonGameCircleExampleWSSyncableNumber> syncableNumbers = null;

    // This is an easy interface to handle each accumulating number type.
    private List<AmazonGameCircleExampleWSAccumulatingNumber> accumulatingNumbers = null;

    // This is an easy interface to handle each syncable number list type.
    private List<AmazonGameCircleExampleWSNumberList> syncableNumberLists = null;

    // This displays the Whispersync hash sets.
    private AmazonGameCircleExampleWSHashSets hashSets = null;

    // This displays the example developer string
    private AmazonGameCircleExampleWSDeveloperString developerString = null;

    // a local reference to the game data map used by Whispersync to access Whispersync save information.
    private AGSGameDataMap dataMap = null;
    #endregion

    #region Local const strings
    // The title of this menu
    private const string whispersyncMenuTitle = "Whispersync";
    // the label for the subsection of this menu containing syncable numbers.
    private const string syncableNumbersLabel = "Syncable Numbers";
    // The label for the subsection of this menu containing accumulating numbers.
    private const string accumulatingNumbersLabel = "Accumulating Numbers";
    // synchronizes whispersync data with the server.
    private const string syncDataButtonLabel = "Synchronize";
    // flushes out whispersync data.
    private const string flushButtonLabel = "Flush";
    // Label for when cloud data has not yet been received.
    private const string noCloudDataReceivedLabel = "No cloud data received.";
    // label for how long ago cloud data was received (in seconds)
    private const string cloudDataLastReceivedLabel = "Received cloud data {0,5:N1} seconds ago.";
    // label for how long ago data was uploaded to cloud
    private const string uploadedToCloudLabel = "Uploaded cloud data {0,5:N1} seconds ago.";
    // label for how long ago since throttle
    private const string lastThrottledLabel = "Throttled {0,5:N1} seconds ago.";
    // label for how long ago since disk write
    private const string diskWriteCompleteLabel = "Disk write complete {0,5:N1} seconds ago.";
    // label for how long ago since first synchronized
    private const string firstSynchronizeLabel = "First synchronize {0,5:N1} seconds ago.";
    // label for how long ago since first synchronized
    private const string alreadySychronizedLabel = "Already synchronized called {0,5:N1} seconds ago.";
    // label for how long ago since first synchronized
    private const string syncFailedLabel = "Sync failed {0,5:N1} seconds ago. Reason: {1}";
    // label for the hash sets
    private const string hashSetsLabel = "Hash Sets";
    // label for the list of syncable number lists
    private const string numberListsLabel = "Syncable Number Lists";
    // label for the developer string
    private const string developerStringLabel = "Developer String";

    // If the whispersync data is null, display this message
    private const string whispersyncUnavailableLabel = "No Whispersync data available.";
    #endregion

    #region Instance management

    // Singleton instance.
    private static AmazonGameCircleExampleWhispersync instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonGameCircleExampleWhispersync"/> class.
    /// </summary>
    private AmazonGameCircleExampleWhispersync() {
        InitSyncableNumbers();
        InitSyncableNumberLists();
        InitAccumulatingNumbers();
        InitHashSets();
        InitDeveloperString();
    }

    public static AmazonGameCircleExampleWhispersync Instance () {
        if (instance == null) {
            instance = new AmazonGameCircleExampleWhispersync();
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
        return whispersyncMenuTitle;
    }
    /// <summary>
    /// Draws the menu. Note that this must be called from an OnGUI function.
    /// </summary>
    public override void DrawMenu() {
        // if cloud data has been received, show how long ago that was.
        if(lastOnNewCloudData.HasValue) {
            double timeElapsed = (System.DateTime.Now - lastOnNewCloudData.Value).TotalSeconds;
            AmazonGUIHelpers.CenteredLabel(string.Format(cloudDataLastReceivedLabel, timeElapsed));
        }

        if(lastOnDataUploadedToCloud.HasValue) {
            double timeElapsed = (System.DateTime.Now - lastOnDataUploadedToCloud.Value).TotalSeconds;
            AmazonGUIHelpers.CenteredLabel(string.Format(uploadedToCloudLabel, timeElapsed));
        }

        if (lastOnThrottled.HasValue) {
            double timeElapsed = (System.DateTime.Now - lastOnThrottled.Value).TotalSeconds;
            AmazonGUIHelpers.CenteredLabel(string.Format(lastThrottledLabel, timeElapsed));
        }

        if (lastOnDiskWriteComplete.HasValue) {
            double timeElapsed = (System.DateTime.Now - lastOnDiskWriteComplete.Value).TotalSeconds;
            AmazonGUIHelpers.CenteredLabel(string.Format(diskWriteCompleteLabel, timeElapsed));
        }

        if (lastOnFirstSynchronize.HasValue) {
            double timeElapsed = (System.DateTime.Now - lastOnFirstSynchronize.Value).TotalSeconds;
            AmazonGUIHelpers.CenteredLabel(string.Format(firstSynchronizeLabel, timeElapsed));
        }

        if (lastOnAlreadySynchronized.HasValue) {
            double timeElapsed = (System.DateTime.Now - lastOnAlreadySynchronized.Value).TotalSeconds;
            AmazonGUIHelpers.CenteredLabel(string.Format(alreadySychronizedLabel, timeElapsed));
        }

        if (lastOnSyncFailed.HasValue) {
            double timeElapsed = (System.DateTime.Now - lastOnSyncFailed.Value).TotalSeconds;
            AmazonGUIHelpers.CenteredLabel(string.Format(syncFailedLabel, timeElapsed, failReason));
        }


        else {
            // display a message that cloud data has not been received yet.
            AmazonGUIHelpers.CenteredLabel(noCloudDataReceivedLabel);
        }

        // This button allows the user to synchronize GameCircle data.
        if(GUILayout.Button(syncDataButtonLabel)) {
            AGSWhispersyncClient.Synchronize();
        }

        // a small space to spread things out.
        GUILayout.Label(GUIContent.none);

        // This button allows the user to flush GameCircle data.
        if(GUILayout.Button(flushButtonLabel)) {
            AGSWhispersyncClient.Flush();
        }

        // a small space to spread things out.
        GUILayout.Label(GUIContent.none);

        // try and initialize the Whispersync data map.
        InitializeDataMapIfAvailable();

        if(null == dataMap) {
            // if it couldn't be retrieved, bail out with an error.
            AmazonGUIHelpers.CenteredLabel(whispersyncUnavailableLabel);
            return;
        }

        // draws the available syncable numbers
        DrawSyncableNumbers();

        // draws the available accumulating numbers
        DrawAccumulatingNumbers();

        // draws the available syncable number lists
        DrawSyncableNumberLists();

        // draws the available hash sets.
        DrawHashSets();

        // draws the developer string
        DrawDeveloperString();
    }
    #endregion

    #region private UI functions
    /// <summary>
    /// Draws the list of syncable numbers.
    /// </summary>
    void DrawSyncableNumbers() {
        // Can't draw it if it isn't initialized yet.
        if(null == syncableNumbers) {
            return;
        }
        // Put the area for number synchronization in a box, to visually separate it from other UI.
        GUILayout.BeginVertical(GUI.skin.box);
        // foldouts make menus cleaner.
        syncableNumbersFoldout = AmazonGUIHelpers.FoldoutWithLabel(syncableNumbersFoldout,syncableNumbersLabel);
        if(syncableNumbersFoldout) {
            // Draw test case submenus, for saving numbers in different ways.
            foreach(AmazonGameCircleExampleWSSyncableNumber syncableNumber in syncableNumbers) {
                syncableNumber.DrawGUI(dataMap);
            }
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the list of accumulating numbers.
    /// </summary>
    void DrawAccumulatingNumbers() {
        // Can't draw it if it isn't initialized yet.
        if(null == accumulatingNumbers) {
            return;
        }
        // A box around this area of the menu helps visually separate it in the UI.
        GUILayout.BeginVertical(GUI.skin.box);
        // foldouts make menus cleaner.
        accumulatingNumbersFoldout = AmazonGUIHelpers.FoldoutWithLabel(accumulatingNumbersFoldout,accumulatingNumbersLabel);
        if(accumulatingNumbersFoldout) {
            // Draw test case submenus, for saving numbers in different ways.
            foreach(AmazonGameCircleExampleWSAccumulatingNumber accumulatingNumber in accumulatingNumbers) {
                accumulatingNumber.DrawGUI(dataMap);
            }
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the syncable number lists.
    /// </summary>
    void DrawSyncableNumberLists() {
        // Can't draw it if it isn't initialized yet.
        if(null == syncableNumberLists) {
            return;
        }
        // A box around this area of the menu helps visually separate it in the UI.
        GUILayout.BeginVertical(GUI.skin.box);
        // foldouts make menus cleaner.
        syncableNumberListsFoldout = AmazonGUIHelpers.FoldoutWithLabel(syncableNumberListsFoldout,numberListsLabel);
        if(syncableNumberListsFoldout) {
            // Draw test case submenus, for saving numbers in different ways.
            foreach(AmazonGameCircleExampleWSNumberList numberList in syncableNumberLists) {
                numberList.DrawGUI(dataMap);
            }
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the hash sets.
    /// </summary>
    void DrawHashSets() {
        // make sure the hashSets have been initialized
        if(null == hashSets) {
            return;
        }

        // Put this box, to visually separate it from other UI.
        GUILayout.BeginVertical(GUI.skin.box);
        // foldouts make menus cleaner.
        hashSetsFoldout = AmazonGUIHelpers.FoldoutWithLabel(hashSetsFoldout,hashSetsLabel);
        if(hashSetsFoldout) {
            // display the hash sets
            hashSets.DrawGUI(dataMap);
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the developer string.
    /// </summary>
    void DrawDeveloperString() {
        if(null == developerString) {
            return;
        }

        // Put this box, to visually separate it from other UI.
        GUILayout.BeginVertical(GUI.skin.box);
        // foldouts make menus cleaner.
        developerStringFoldout = AmazonGUIHelpers.FoldoutWithLabel(developerStringFoldout,developerStringLabel);
        if(developerStringFoldout) {
            developerString.DrawGUI(dataMap);
        }
        GUILayout.EndVertical();
    }
    #endregion

    #region private functions
    /// <summary>
    /// Initializes the data map if available.
    /// </summary>
    void InitializeDataMapIfAvailable() {
        // if the data map was already retrieved, nothing to do here.
        if(null != dataMap) {
            return;
        }
        dataMap = AGSWhispersyncClient.GetGameData();

        // if the data map was retrieved for the first time, do
        // any first-time initialization behavior, such as subscribing
        // to callbacks.
        if(null != dataMap) {
            // subscribe to the new cloud data event.
            AGSWhispersyncClient.OnNewCloudDataEvent += OnNewCloudData;
            AGSWhispersyncClient.OnDataUploadedToCloudEvent += OnDataUploadedToCloud;
            AGSWhispersyncClient.OnThrottledEvent += OnThrottled;
            AGSWhispersyncClient.OnDiskWriteCompleteEvent += OnDiskWriteComplete;
            AGSWhispersyncClient.OnFirstSynchronizeEvent += OnFirstSynchronize;
            AGSWhispersyncClient.OnAlreadySynchronizedEvent += OnAlreadySynchronized;
            AGSWhispersyncClient.OnSyncFailedEvent += OnSyncFailed;
        }
    }

    /// <summary>
    /// Initializes syncable numbers used by Whispersync.
    /// </summary>
    void InitSyncableNumbers() {
        // No need to re-initialize if it's already been initialized.
        if(null != syncableNumbers) {
            return;
        }
        // To properly test Whispersync, every possible test case needs to be available.
        // This is every syncable number behavior matched against every syncable number type.
        syncableNumbers = new List<AmazonGameCircleExampleWSSyncableNumber>();

        // Get a list of enum values of the syncable number behaviors.
        System.Array syncableNumberBehaviors =
            System.Enum.GetValues(typeof(AmazonGameCircleExampleWSSyncableNumber.SyncableNumberBehavior));
        // Get a list of enum values of the syncable number types.
        System.Array availableSyncableNumberTypes =
            System.Enum.GetValues(typeof(AmazonGameCircleExampleWSSyncableNumber.AvailableSyncableNumberType));

        // For every syncable number behavior...
        foreach(AmazonGameCircleExampleWSSyncableNumber.SyncableNumberBehavior behavior in syncableNumberBehaviors) {
            // and every syncable number type...
            foreach(AmazonGameCircleExampleWSSyncableNumber.AvailableSyncableNumberType numberType in availableSyncableNumberTypes) {
                // add to the list of syncable numbers to be displayed.
                syncableNumbers.Add(new AmazonGameCircleExampleWSSyncableNumber(behavior,numberType));
            }
        }

    }

    /// <summary>
    /// Inits the list of syncable number lists.
    /// </summary>
    void InitSyncableNumberLists() {
        // No need to re-initialize if it's already been initialized.
        if(null != syncableNumberLists) {
            return;
        }
        // To properly test Whispersync, every possible test case needs to be available.
        // This is every syncable number list type.
        syncableNumberLists = new List<AmazonGameCircleExampleWSNumberList>();

        // Get a list of enum values of the number list types.
        System.Array availableListTypes =
            System.Enum.GetValues(typeof(AmazonGameCircleExampleWSNumberList.AvailableListType));

        foreach(AmazonGameCircleExampleWSNumberList.AvailableListType listType in availableListTypes) {
            // add to the list of number lists to be displayed.
            syncableNumberLists.Add(new AmazonGameCircleExampleWSNumberList(listType));
        }
    }

    /// <summary>
    /// Inits the list of accumulating numbers.
    /// </summary>
    void InitAccumulatingNumbers() {
        // No need to re-initialize if it's already been initialized.
        if(null != accumulatingNumbers) {
            return;
        }
        // To properly test Whispersync, every possible test case needs to be available.
        // This is every accumulating number type.
        accumulatingNumbers = new List<AmazonGameCircleExampleWSAccumulatingNumber>();

        // Get a list of enum values of the accumulating number types.
        System.Array availableAccumulatingNumberTypes =
            System.Enum.GetValues(typeof(AmazonGameCircleExampleWSAccumulatingNumber.AvailableAccumulatingNumberType));

        foreach(AmazonGameCircleExampleWSAccumulatingNumber.AvailableAccumulatingNumberType numberType in availableAccumulatingNumberTypes) {
            // add to the list of syncable numbers to be displayed.
            accumulatingNumbers.Add(new AmazonGameCircleExampleWSAccumulatingNumber(numberType));
        }
    }

    /// <summary>
    /// Initializes the hash sets.
    /// </summary>
    void InitHashSets() {
        // No need to re-initialize if it's already been initialized.
        if(null != hashSets) {
            return;
        }
        hashSets = new AmazonGameCircleExampleWSHashSets();
    }

    /// <summary>
    /// Initializes the developer string.
    /// </summary>
    void InitDeveloperString() {
        if(null != developerString) {
            return;
        }
        developerString = new AmazonGameCircleExampleWSDeveloperString();
    }
    #endregion

    #region Callbacks
    /// <summary>
    /// Callback when Whispersync has new cloud data available.
    /// </summary>
    private void OnNewCloudData() {
        lastOnNewCloudData = System.DateTime.Now;
    }

    private void OnDataUploadedToCloud() {
        lastOnDataUploadedToCloud = System.DateTime.Now;
    }

    private void OnThrottled() {
        lastOnThrottled = System.DateTime.Now;
    }

    private void OnDiskWriteComplete() {
        lastOnDiskWriteComplete = System.DateTime.Now;
    }

    private void OnFirstSynchronize() {
        lastOnFirstSynchronize = System.DateTime.Now;
    }

    private void OnAlreadySynchronized() {
        lastOnAlreadySynchronized = System.DateTime.Now;
    }

    private void OnSyncFailed() {
        lastOnSyncFailed = System.DateTime.Now;
        failReason = AGSWhispersyncClient.failReason;
    }
    #endregion
}
