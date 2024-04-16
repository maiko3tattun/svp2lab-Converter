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

function main() {
  var scope = SV.getMainEditor().getCurrentGroup();
  var group = scope.getTarget();
  var phonemes = SV.getPhonemesForGroup(scope);
  var timeAxis = SV.getProject().getTimeAxis();

  for(var i = 0; i < group.getNumNotes(); i ++) {
    if (phonemes[i] === "") {
      var results = SV.showOkCancelBox("Warning", "Voice Unselected or No Phoneme")
      if (results) {
        break;
      } else {
        SV.finish();
      }
    }
  }

  var preEnd = 0;
  var output = "BPM: " + timeAxis.getTempoMarkAt(scope.getOnset()).bpm
   + "\r\nNotes:\r\n";

  for(var i = 0; i < group.getNumNotes(); i ++) {
    var note = group.getNote(i);

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

    preEnd = note.getEnd()
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
  SV.finish();
}
