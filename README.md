# svp2lab Converter - DiffSinger・NNSVSラベリング支援システム

## これは何？
DiffSingerのラベリングを、譜面ない！labの叩き台もない！wav以外なにもない！の状態から最速で終わらせる方法を模索した結果、Synthesizer V 有料版エディタの譜面起こし機能を使ったチートシステムを構築した。  

デモンストレーション：https://twitter.com/maiko3tattun/status/1780263803219054866

## DiffSinger ラベル生成ツールまとめ
1. SynthVの譜面をlabにするツール（これ）
   - メリット：譜面起こし時点でq（喉切り）が認識される・母音の位置は一致するはずなのでほぼ直さなくてよい・ENUNU用のustが出せる
   - デメリット：濁音の判別精度が微妙・早口がダメ・子音位置は手動調整になる
2. 歌詞を音素にそのまま変換してSOFAで自動ラベリング（[歌詞変換 for 歌声合成ソフト](https://ameblo.jp/maiko3utau/entry-12660767498.html)＋[LabelMakr](https://github.com/spicytigermeat/LabelMakr)）
   - メリット：歌詞の誤検出をほぼ気にしなくてよい
   - デメリット：q（喉切り）、無声化、「っ」の挿入を手動で編集する必要がありけっこう手間
   - ラベル位置の精度は1と大して変わらない
3. SynthVで譜面起こしした歌詞を音素に変換してSOFAで自動ラベリング（[スクリプト](https://x.com/maiko3tattun/status/1806633768389955743)＋[LabelMakr](https://github.com/spicytigermeat/LabelMakr)）
   - メリット：譜面起こし時点でq（喉切り）、無声化、「っ」が認識される
   - デメリット：譜面起こし時点での濁音の判別精度が微妙・早口がダメ
   - ラベル位置の精度は1と大して変わらない
4. 歌詞テキストにqを手動でちゃちゃっと足すツール（準備中）＋SOFAで自動ラベリング

## サポート環境
- Win10以降 64bit
- macOS 10.12以降 64bit（検証報告ください）
- Synthesizer V Studio Pro 1.11.0以降
- 日本語曲で検証済み。理論上英語と中国語も可能（`replace config`要編集）。フィードバック求む。
- SynthVスクリプトとコンソールアプリのセットです。mac対応を優先した結果GUIがなくなった。

Download: https://github.com/maiko3tattun/svp2lab-Converter/releases

## ワークフローの概要
1. 歌を録る（ENUNUの場合は補正もする）
2. SynthVのボーカルMIDI変換で譜面起こしする・歌詞の手動修正
3. 同梱のSynthV scriptでノートをクリップボードにコピーする
4. svp2lab Converterを使ってlabを生成　※音素はノートの歌詞からの変換ではなく、SynthVの音素を使います
5. vLabelerでラベリング
6. ENUNU用のustエクスポートにも対応。歌詞に音素直接表記なので他ツールとの相性が良くないかもしれない

## 有用性
- SynthVの譜面起こしの精度が8割ぐらいなので、譜面作成=lab叩き台作成の手間が8割省ける
- 変換後のlabは母音がほぼ正確・子音位置は決め打ち（デフォルト値編集可能）で、vLabelerで子音だけ直すとして、ラベリングの半分以上の手間が省ける
- UTAUの連呼式CVVCや歌連続音を流用するケースでは、フレーズ中に同じ音程が複数回出現する場合 wav中にエイリアスのない音符が含まれるため、ini→lab変換ソフトを使うよりこちらの方が早い可能性がある

## 操作マニュアル
0. インストール
   - `svp2lab Converter`のzipを適当な場所に展開。フリーソフトはProgramFilesに入れちゃだめって古事記にも書いてある。
   - `CopyNotesForLabelConverter.js`をSynthVのスクリプトフォルダにインストール。
   - 他、便利なスクリプトをインストールしていると尚良し。うちのだと[「選択したノートを分割・歌詞も分割」「選択したノートをマージPlus(歌詞はマージしないver.)」「隙間にブレスを入れる」](https://drive.google.com/drive/folders/13YUromADAUrgNrRqJ8k7qAja627rYjXG?usp=sharing)あたりは必須。
1. SynthVプロジェクトを作って歌のwavをインポートする。テンポを設定・wav開始位置を拍に合わせる（見やすいので）。
   - 歌は `音源名フォルダ > 表情名フォルダ > 曲名フォルダ > "wav"フォルダ`に入れておくとよい
   - wavは複数入れてもok。wav分だけボーカルトラックを作るか、1トラックに変換後のノートグループが重ならないで並ぶようにwavの位置をずらしておく
2. ボーカルMIDI変換する。歌詞を解析にチェック。ノート検出感度は100%でよし。他は適当。
   - 操作中のトラックにwavと同じ長さのノートグループが作られるが、ノートグループは操作感が特殊なので慣れてない場合は公式マニュアル参照。
   - ブレスは検知してくれないので、DiffSingerの場合はスクリプトでブレスを一括挿入しておく。[Add Breath配布所](https://forum.synthesizerv.com/t/topic/11670)
3. 譜面の手直し
   - 歌詞の修正と、早口の場合ノートが合体していることがあるのでそれを分割する。wavの音質次第だがノートの位置と長さは割と合っているはず。
   - ノートの歌詞は（今のところ）使わないので適当で良い。音素が合ってることが大事。
   - トラックにボイスが読み込まれてないと歌詞→音素の変換が発生しないので必ず設定すること。
   - 仕様上「っ」と「'」がどちらもclになるので、後の段階で変換する。「'あ」の場合、clとaは同じノートにあったほうが後で楽。
   - ブレスを一括挿入した場合は、ブレスがない部分のbrノートを削除しておく
   - ノートを増やす場合、トラックエリアを横にズームしてwavを見ながらノート分割する位置を探ると良い
   - Ctrl+Altでスナップせずにノート長を変えられる
4. `CopyNotesForLabelConverter`スクリプトでノートをクリップボードにコピーする
   - 日本語名は「svp2lab Converter用にノートをコピー」。デフォルトではOtherカテゴリに入っている。
   - トラックにボイスが読み込まれてない、または歌詞→音素変換に失敗しているノートがある場合、警告が出る。トラックにボイスがある場合はそのまま確定を押せば進める。
5. `svp2lab Converter`を起動
   - Winでは`svp2lab Converter.exe`、Macではなんかターミナルのアイコンのやつ？のはず
   - セキュリティ警告が出た場合はなんかうまいことやってくれ
6. `Paste from "Copy Notes for Label Converter" script:`　→さっきコピーしたやつを貼って、Enterを押す
7. `Replace Config Path:` →同梱の`replace config～.txt`をドラッグアンドドロップして、Enterを押す
   - 音源によって音素表記が違うので、置換表を使う
   - 現状、DiffSinger用とNNSVS用を同梱。中身は編集可。Tab区切りで書けば部分一致で置換される。
8. `Length Config Path:` →同梱の`length config.txt`をドラッグアンドドロップして、Enterを押す
   - 子音長データ。Tab区切り・ミリ秒表記
   - replace config と Length Config は指定しなくても動作可能だが、ぐちゃぐちゃになるので非推奨
9. `Wav Path:` →wavをドラッグアンドドロップして、Enterを押す
10. "Output Succeed!" と表示されてwavの隣に`.lab`ファイルができる
11. NNSVSの場合でustが必要であれば、`ustも出す？ y/n`に`y`と入力してEnterを押す。wavの隣に`.ust`ファイルができる
12. 続けてlab変換する場合はEnterを押すと6.に戻れる
13. wavフォルダの隣にlabフォルダを作って、labファイルを移動
14. vLabelerで曲名フォルダ（wavフォルダとlabフォルダが入ってるフォルダ）を選択してNNSVSラベラーのプロジェクトを作成。あとは普通にラベリング。

## Welcome Contribute!
- mac対応、英語・中国語対応が中途半端
- 深夜テンションで作ったのでノリがおかしい・コンソールの多言語化が中途半端

## 利用規約／Terms of Use
- このツールを使用してSynthesizer Vのキャラクターの音声を学習させる等、趣旨に反する使い方は禁止します。
- 本ツール・スクリプトを利用して発生した問題については一切責任を負いかねますのでご了承下さい。
- 不具合報告はTwitter、メール、Discord、ここのIssue等に投げてください。サポート環境下であればできる限り対処しますので遠慮なくご連絡ください。

### 作った人
まいこ
- Twitter: https://twitter.com/maiko3tattun
- HP: https://maiko3tattun.wixsite.com/mysite
- mail: maikotattun @yahoo.co.jp
