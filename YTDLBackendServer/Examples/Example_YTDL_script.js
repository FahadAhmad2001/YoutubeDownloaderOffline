// JavaScript source code
// Some code was obtained from https://www.html5rocks.com/en/tutorials/cors/ , https://bytenota.com/parsing-an-xml-file-in-javascript/ ,  and StackOverflow
//You can set your constants here
var serverUrl = "http://127.0.0.1:9000/"; //IP/URL of the backend server
function ProcessMetadata(MetadataInfo) {
    if (window.DOMParser) {
        parser = new DOMParser();
        xmlDoc = parser.parseFromString(MetadataInfo, "text/xml");
    }
    else // Internet Explorer
    {
        xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        xmlDoc.async = false;
        xmlDoc.loadXML(MetadataInfo);
    }
    document.getElementById("VidTitle").innerHTML = xmlDoc.getElementsByTagName("Title")[0].childNodes[0].nodeValue;
    document.getElementById("Thumbnail").src = xmlDoc.getElementsByTagName("ThumbnailURL")[0].childNodes[0].nodeValue;
    var selectVid = document.getElementById("Videos");
    var selectAud = document.getElementById("Audios");
    var VidQualitiesList = xmlDoc.getElementsByTagName("VideoQualities")[0];
    for (var i = 0; i < VidQualitiesList.childElementCount; i++) {
        var CurrentVidQual = VidQualitiesList.childNodes[i];
        var opt = document.createElement('option');
        opt.value = CurrentVidQual.attributes["resolution"].value + " - " + CurrentVidQual.attributes["size"].value + " - " + CurrentVidQual.attributes["format"].value + " ID: " + CurrentVidQual.attributes["ID"].value;
        opt.innerHTML = CurrentVidQual.attributes["resolution"].value + " - " + CurrentVidQual.attributes["size"].value + " - " + CurrentVidQual.attributes["format"].value + " ID: " + CurrentVidQual.attributes["ID"].value;
        selectVid.appendChild(opt);
    }
    var AudQualitiesList = xmlDoc.getElementsByTagName("AudioQualities")[0];
    for (var i = 0; i < AudQualitiesList.childElementCount; i++) {
        var CurrentAudQual = AudQualitiesList.childNodes[i];
        var opt = document.createElement('option');
        opt.value = CurrentAudQual.attributes["format"].value + " - " + CurrentAudQual.attributes["size"].value + " ID: " + CurrentAudQual.attributes["ID"].value;
        opt.innerHTML = CurrentAudQual.attributes["format"].value + " - " + CurrentAudQual.attributes["size"].value + " - ID: " + CurrentAudQual.attributes["ID"].value;;
        selectAud.appendChild(opt);
    }
    var opt = document.createElement('option');
    opt.value = "MP3 Only";
    opt.innerHTML = "MP3 Only";
    selectVid.appendChild(opt);
    document.getElementById("MetadataButton").value = "Get Metadata";
}
function DownloadVideo() {
    document.getElementById("DownloadVideoButton").value = "Please wait"
    var VidDropDown = document.getElementById("Videos");
    var AudDropDown = document.getElementById("Audios");
    var selectedVid = VidDropDown.options[VidDropDown.selectedIndex].value;
    var VidURL = document.getElementById("VidURL").value;
    if (selectedVid == "MP3 Only") { //MP3 Only
        var isChecked = document.getElementById("UseThumbnail").checked;
        if (isChecked) {
            var VidID = VidURL.split("watch?v=")[1];
            var url = serverUrl + "downloadvid?vidID=" + VidID + "&isMP3=true&useThumbnail=true";
            makeCorsRequest(url);
        }
        else {
            var VidID = VidURL.split("watch?v=")[1];
            var url = serverUrl + "downloadvid?vidID=" + VidID + "&isMP3=true&useThumbnail=false";
            makeCorsRequest(url);
        }
    }
    else {
        var selectedAud = AudDropDown.options[AudDropDown.selectedIndex].value;
        var selVid = selectedVid.split("ID: ")[1];
        var selAud = selectedAud.split("ID: ")[1];
        var selVidFormat = selectedVid.split(" ID: ")[0].split(" - ")[2];
        var selAudFormat = selectedAud.split(" - ")[0];
        if ((selAud.includes("m4a") && selVid.includes("webm")) || selAud.includes("webm") && selVid.includes("mp4")) {
            alert("Please use either webm only or use mp4 and m4a");
        }
        else {
            var VidID = VidURL.split("watch?v=")[1];
            var url = serverUrl + "downloadvid?vidID=" + VidID + "&isMP3=false&videoID=" + selVid + "&audioID=" + selAud + "&videoFormat=" + selVidFormat + "&audioFormat=" + selAudFormat;
            makeCorsRequest(url);
        }

    }
}
function GetVideoMetadata() {
    document.getElementById("MetadataButton").value = "Loading...";
    removeOptions(document.getElementById("Videos"));
    removeOptions(document.getElementById("Audios"));
    var VidURL = document.getElementById("VidURL").value;
    var url = serverUrl + "getmetadata?vidID=" + VidURL.split("watch?v=")[1];
    makeCorsRequest(url);

}
function makeCorsRequest(URL) {
    // This is a sample server that supports CORS.
    var url = URL;
    var oldProgress;
    var xhr = createCORSRequest('GET', url);
    if (!xhr) {
        alert('CORS not supported');
        return;
    }

    // Response handlers.
    xhr.onload = function () {
        var text = xhr.responseText;
        if (url.includes("/getmetadata?vidID=")) {
            ProcessMetadata(text);
        }
        if (url.includes("/downloadvid?vidID=")) {
            if (text != "") {
                //document.getElementById("ProgressText").innerHTML = text;
                var fileLink = text.split("COMPLETED:")[1];
                document.getElementById("ProgressText").innerHTML = "";
                var linkURL = serverUrl + "download?file=" + fileLink;
                //document.getElementById("ProgressText").setAttribute('href',linkURL);
                var a = document.createElement('a');
                a.setAttribute('href', linkURL);
                a.innerHTML = "Direct Download Link (for 4 hours)";
                document.getElementById('ProgressText').appendChild(a);
                document.getElementById("DownloadVideoButton").value = "Download";
            }
        }
    };
    xhr.onprogress = function () {
        var text = xhr.responseText;
        if (url.includes("/downloadvid?vidID=")) {
            if (text != "") {
                var textToShow = text.split(oldProgress)[1];
                document.getElementById("ProgressText").innerHTML = textToShow;
                oldProgress = text;
            }
        }
    }

    xhr.onerror = function () {
        alert('Woops, there was an error making the request.');
    };

    xhr.send();
}
function removeOptions(selectbox) {
    var i;
    for (i = selectbox.options.length - 1; i >= 0; i--) {
        selectbox.remove(i);
    }
}
function createCORSRequest(method, url) {
    var xhr = new XMLHttpRequest();
    if ("withCredentials" in xhr) {
        // XHR for Chrome/Firefox/Opera/Safari.
        xhr.open(method, url, true);
    } else if (typeof XDomainRequest != "undefined") {
        // XDomainRequest for IE.
        xhr = new XDomainRequest();
        xhr.open(method, url);
    } else {
        // CORS not supported.
        xhr = null;
    }
    return xhr;
}

// Helper method to parse the title tag from the response.
function getTitle(text) {
    return text;
}