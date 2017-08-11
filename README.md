# ProceduralTerrainUnity

This is the unity project part of the terrain stitching

Full instructions
READ ME
This is the full Terrain Stitching Project with implementation using IP Webcam and VLC 
To use: 
	Adjust the different terrain height and width parameters and chunk size, this is up to the user, though testing has proven the headset maxes out around 1000x1000 at chunk size 50, but users can test this for themselves with the inspector adjustments. 
	 Run IP Webcam with video resolution 960x720 
	Turn off audio
	Portrait mode 
Start VLC 
To run: 
	Got to tools>preferences
	In the bottom left under show settings click all.
	Go down to Video and under it Filters.
	Make sure scene video filters is checked. 
	Expand the filters check. 
	Go down to scene filter.
	Make sure format is png. 
	Image width is 960. 
	Image height isv1280. 
	Recording ratio is 5. 
	Filename prefix has to be images. 
	Check always write to same file. 
Set directory prefix to > C:\Users\swri11\Documents\GitHub\VLCImages
This prefix will only work on this computer if the Unity project location is not changed
	After that start the rtsp stream. 
	Go to media>opennetworkstream>network tab	
	The network Url is: http://10.202.17.41:8080/video on this network though it can change, just check what IP Webcam says at the bottom. 
	Hit play. 
	Double-check that an image is being created in the scenes VLCImages folder 
Start the TerrainStitching OpenCV project: 
	Navigate to the C:\Users\swri11\Documents\GitHub\TerrainStitchingProject\TestStitching\x64\Release folder in the command terminal
	run TerrainStitching.exe 
	(to run without IP webcam, run this application but add a video name in the folder at the end example: test.mp4, test2.mp4, test4.mp4)
Start the Unity project. 
	Open the scene labelled test. 
	Play. 
       
