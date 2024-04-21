function getClientInfo() {
  return {
    "name" : SV.T("Copy Notes for svp2lab Converter"),
    "author" : "Maiko",
    "versionNumber" : 1.00,
    "minEditorVersion" : 68352,
    "category" : "Others"
  };
}

function getTranslations(langCode) {
  if(langCode == "ja-jp") {
    return [
      ["Copy Notes for svp2lab Converter", "svp2lab Converter用にノートをコピー"]
    ];
  }
  return [];
}

function output(){
  var scope = SV.getMainEditor().getCurrentGroup();
  var group = scope.getTarget();
  var phonemes = SV.getPhonemesForGroup(scope);
  var timeAxis = SV.getProject().getTimeAxis();

  if (group.getNumNotes() == 0){
    SV.showMessageBox("Warning", "No Notes or No Group Selected");
    return;
  }

  var preEnd = 0;
  var output = "BPM: " + timeAxis.getTempoMarkAt(scope.getOnset()).bpm
    + "\r\nNotes:\r\n";

  var flag = true;
  for(var i = 0; i < group.getNumNotes(); i ++) {
    var note = group.getNote(i);

    if (note.getLyrics() != "-" && phonemes[i] == "" && flag) {
      var results = SV.showOkCancelBox("Warning", "Voice Unselected or No Phoneme");
      if (results) {
        flag = false;
      } else {
        return;
      }
    }

    if(note.getOnset() > preEnd){
      output += "- Onset: " + preEnd
      + "\r\n  OnsetSec: " + timeAxis.getSecondsFromBlick(preEnd)
      + "\r\n  Lyric: R"
      + "\r\n  Phonemes: R"
      + "\r\n  Tone: 60"
      + "\r\n";
    }

    output += "- Onset: " + note.getOnset()
    + "\r\n  OnsetSec: " + timeAxis.getSecondsFromBlick(note.getOnset())
    + "\r\n  Lyric: \"" + note.getLyrics() + "\""
    + "\r\n  Phonemes: " + phonemes[i]
    + "\r\n  Tone: " + note.getPitch()
    + "\r\n";

    preEnd = note.getEnd();
  }

  output += "- Onset: " + preEnd
  + "\r\n  OnsetSec: " + timeAxis.getSecondsFromBlick(preEnd)
  + "\r\n  Lyric: R"
  + "\r\n  Phonemes: R"
  + "\r\n  Tone: 60"
  + "\r\nLengthSec: " + timeAxis.getSecondsFromBlick(scope.getDuration())
  + "\r\n";

  SV.setHostClipboard(output);
  SV.showMessageBox("", "Copy succeed!");
}

function main() {
  output();
  SV.finish();
}