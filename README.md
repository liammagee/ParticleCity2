

## Importing tile maps into unity

Reference:

http://alastaira.wordpress.com/2013/11/12/importing-dem-terrain-heightmaps-for-unity-using-gdal/

Download GDAL utilities:

General: http://trac.osgeo.org/gdal/wiki/DownloadingGdalBinaries

For Mac: http://www.kyngchaos.com/software:frameworks

GDAL Docs:

http://www.gdal.org/gdal_translate.html

Mac DEM Viewer:



Download tile maps from:

http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Australia/

Or:

http://ws.csiss.gmu.edu/DEMExplorer/


Merge multiple tiles (for Melbourne):

gdal_merge.py -o Melbourne.hgt S38E144.hgt S38E145.hgt S39E144.hgt S39E145.hgt


Translate to a TIF (DEM format)

gdal_translate Melbourne.hgt Melbourne.tif

Prepare for Unity:

 gdal_translate -ot UInt16 -scale -of ENVI -outsize 1025 1025 Melbourne.hgt Melbourne.raw


Unity Notes:

http://forum.unity3d.com/threads/23851-importing-real-maps-(DEMs)-into-unity

General Terrain mmapping references

http://en.wikipedia.org/wiki/Shuttle_Radar_Topography_Mission

http://www.ga.gov.au/metadata-gateway/metadata/record/gcat_66006
http://www.webgis.com/srtm3.html
http://dds.cr.usgs.gov/srtm/version2_1/Documentation/Quickstart.pdf
