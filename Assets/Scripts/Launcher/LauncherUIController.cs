﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls all aspects of the launcher UI.  Creates and sorts the playlists (which will have their own controllers)
/// </summary>
public class LauncherUIController : MonoBehaviour
{

    public List<Playlist> playlistData;

    public GameObject playlistsContainer;                 // Object that marks the position where the current selected playlist should be placed
    public GameObject playlistLabelsContainer;             // Object that marks the position where the playlist name labels will be placed

    public GameObject playlistUIControllerPrefab;
    public GameObject playlistLabelPrefab;

    public Animation arrowLeft;
    public Animation arrowRight;

    public float tweenTime;                             // Time for movement of playlist and playlist labels

    public Vector3 offsetPlaylists;
    public Vector3 offsetPlaylistLabels;

    public float smallScale = 0.7f;

    public List<PlaylistLabel> playlistLabelList;       // List of all playlist labels
    public List<PlaylistUIController> playlistControllers;

    public int selectedPlaylistIndex;
    

    void Start()
    {
        playlistData = GM.data.playlists;
        BuildPlaylists();
    }

    void Update()
    {
        if (Input.GetKeyDown(GM.options.keys.P1Left))
            PreviousPlaylist();

        if (Input.GetKeyDown(GM.options.keys.P1Right))
            NextPlaylist();

        if (Input.GetKeyDown(GM.options.keys.P1Up))
            PreviousGame();

        if (Input.GetKeyDown(GM.options.keys.P1Down))
            NextGame();

        if (Input.GetKeyDown(GM.options.keys.P1Button1))
            SelectGame();

    }

    private void BuildPlaylists()
    {

        foreach (var playlist in playlistData)
        {

            // Instantiate a new playlist and set the path to its directory
            GameObject newPlaylistController = Instantiate(playlistUIControllerPrefab, playlistsContainer.transform, true) as GameObject;
            PlaylistUIController newPlaylist = newPlaylistController.GetComponent<PlaylistUIController>();
            newPlaylist.playlist = playlist;
            newPlaylist.Init();
            playlistControllers.Add(newPlaylist);

            GameObject newPlaylistLabel = Instantiate(playlistLabelPrefab, playlistLabelsContainer.transform, true) as GameObject;
            newPlaylistLabel.GetComponent<PlaylistLabel>().name = "PlaylistLabel: " + playlist.name;
            newPlaylistLabel.GetComponent<PlaylistLabel>().initializeName(playlist.name);
            playlistLabelList.Add(newPlaylistLabel.GetComponent<PlaylistLabel>());

        }

        // Check whether there is more than one playlist, if there is only one, deactivate the arrow graphics on either side of the label
        if (playlistData.Count <= 1)
        {
            arrowRight.gameObject.SetActive(false);
            arrowLeft.gameObject.SetActive(false);
        }

        RepositionPlaylists();
    }

    private void RepositionPlaylists()
    {
        var i = 0;
        foreach(var playlist in playlistControllers)
        {
            var relativeIndex = i - selectedPlaylistIndex;
            var thisOffset = offsetPlaylists * relativeIndex;

            //Position
            Vector3 newPosition = playlistsContainer.transform.position + thisOffset;
            
            //Scale
            playlist.transform.localScale = new Vector3(1, 1, 1);

            //Commit
            playlist.TweenPosition(newPosition);

            i++;
        }

        i = 0;
        foreach (var playlistLabel in playlistLabelList)
        {
            var relativeIndex = i - selectedPlaylistIndex;
            var thisOffset = offsetPlaylistLabels * relativeIndex;

            //Position
            Vector3 newPosition = thisOffset;

            //Scale
            playlistLabel.transform.localScale = new Vector3(1, 1, 1);

            //Commit
            playlistLabel.TweenLocalPosition(newPosition);

            i++;
        }
    }

    private void NextPlaylist()
    {
        selectedPlaylistIndex++;

        if (selectedPlaylistIndex >= playlistData.Count)
            selectedPlaylistIndex = 0;

        RepositionPlaylists();
    }

    private void PreviousPlaylist()
    {
        selectedPlaylistIndex--;

        if (selectedPlaylistIndex < 0)
            selectedPlaylistIndex = playlistData.Count - 1;

        RepositionPlaylists();
    }

    private void NextGame()
    {
        playlistControllers[selectedPlaylistIndex].NextGame();
    }

    private void PreviousGame()
    {
        playlistControllers[selectedPlaylistIndex].PreviousGame();
    }

    private void SelectGame()
    {
        GM.runner.Run(playlistControllers[selectedPlaylistIndex].GetCurrentGame());
    }
}
