using Android.Media;
using Microsoft.AspNetCore.Components;

namespace LanguagePlayer.Pages;

public partial class Index
{
    private int _pageIndex;
    private int _sentenceIndex;
    private int _audioPageIndex;

    private const string LanguageFolder = "/storage/emulated/0/Jazyky/Francais";

    private bool disabled;

    private const int Shifter = 1;
    private string _audioFolder;

    private readonly List<List<string>> _pagesSlovak = new();
    private readonly List<List<string>> _pagesForeign = new();
    private readonly List<List<string>> _pagesForeignWord = new();

    List<string> pages = new();

    private List<string> books = new()
    {
        "5000", "7500", "8500", "10000"
    };

    private string txtSlovak;
    private string txtForeign;
    private string txtPage;
    private string txtSentence;
    private string txtSentencesCount;
    private string txtForeignWord;
    private string selectForeignWord;
    private string selectBook;

    private MediaPlayer _mediaPlayer;

    protected override async Task OnInitializedAsync()
    {
        string generalToken = await SecureStorage.Default.GetAsync("MyKey");

        int book = 10000;

        if (generalToken != null)
        {
            var tokens = generalToken.Split(";");

            _pageIndex = int.Parse(tokens[1]);
            _sentenceIndex = int.Parse(tokens[2]);
            book = int.Parse(tokens[0]);
        }

        txtPage = (_pageIndex + 1).ToString();
        selectForeignWord = (_pageIndex + 1).ToString();
        txtSentence = (_sentenceIndex + 1).ToString();

        selectBook = book.ToString();

        LoadData(book);
    }

    private void LoadData(int book)
    {
        _audioPageIndex = _pageIndex + Shifter;
        _audioFolder = book.ToString();

        var file = _audioFolder + ".txt";
        var path = Path.Combine(LanguageFolder, $"{file}");
        var text = File.ReadAllText(path);
        var splitPages = text.Split("--NEW_PAGE--");

        _pagesSlovak.Clear();
        _pagesForeign.Clear();
        _pagesForeignWord.Clear();
        pages.Clear();

        if (book == 5000)
        {
            foreach (var splitPage in splitPages)
            {
                var pageSlovak = new List<string>();
                var pageForeign = new List<string>();
                var pageForeignWord = new List<string>();
                var splitSentencesAndTranslations = splitPage.Split("\r\n\r\n");
                foreach (var splitSentenceAndTranslation in splitSentencesAndTranslations)
                {
                    var splitSentenceAndTranslationTrim = splitSentenceAndTranslation.TrimStart();
                    if (splitSentenceAndTranslationTrim.Length == 0)
                    {
                        continue;
                    }

                    var slovakSentenceTrim =
                        splitSentenceAndTranslationTrim.Substring(0,
                            splitSentenceAndTranslationTrim.IndexOf("\r\n"));
                    var foreignSentenceTrim =
                        splitSentenceAndTranslationTrim.Substring(splitSentenceAndTranslationTrim.TrimEnd()
                            .LastIndexOf("\r\n"));
                    // todo get rid of  * and -- in foreignSentenceTrim


                    var trimmed = splitSentenceAndTranslationTrim.TrimEnd();
                    var foreignWordTrim = trimmed.Substring(trimmed.IndexOf("\r\n"),
                        trimmed.LastIndexOf("\r\n") - trimmed.IndexOf("\r\n"));

                    pageSlovak.Add(slovakSentenceTrim);
                    pageForeign.Add(foreignSentenceTrim.TrimStart());
                    pageForeignWord.Add(foreignWordTrim.TrimStart());
                }

                _pagesSlovak.Add(pageSlovak);
                _pagesForeign.Add(pageForeign);
                _pagesForeignWord.Add(pageForeignWord);
            }

            for (var i = 1; i < _pagesSlovak.Count; i++)
            {
                pages.Add(i.ToString());
            }
        }
        else if (book == 7500 || book == 8500 || book == 10000)
        {
            foreach (var splitPage in splitPages)
            {
                var pageSlovak = new List<string>();
                var pageForeign = new List<string>();
                var pageForeignWord = new List<string>();
                var splitSentencesAndTranslations = splitPage.Split("\r\n\r\n");
                foreach (var splitSentenceAndTranslation in splitSentencesAndTranslations)
                {
                    var splitSentenceAndTranslationTrim = splitSentenceAndTranslation.TrimStart();
                    if (splitSentenceAndTranslationTrim.Length == 0)
                    {
                        continue;
                    }

                    var slovakSentenceTrim =
                        splitSentenceAndTranslationTrim.Substring(0,
                            splitSentenceAndTranslationTrim.IndexOf("\r\n"));
                    var foreignSentenceTrim =
                        splitSentenceAndTranslationTrim.Substring(splitSentenceAndTranslationTrim.TrimEnd()
                            .LastIndexOf("\r\n"));

                    var trimmed = splitSentenceAndTranslationTrim.TrimEnd();
                    var foreignWordTrim = trimmed.Substring(trimmed.IndexOf("\r\n"),
                        trimmed.LastIndexOf("\r\n") - trimmed.IndexOf("\r\n"));

                    pageSlovak.Add(slovakSentenceTrim);
                    pageForeign.Add(foreignSentenceTrim.TrimStart());
                    pageForeignWord.Add(foreignWordTrim.TrimStart());
                }

                _pagesSlovak.Add(pageSlovak);
                _pagesForeign.Add(pageForeign);
                _pagesForeignWord.Add(pageForeignWord);
            }

            for (var i = 1; i < _pagesSlovak.Count; i++)
            {
                pages.Add(i.ToString());
            }
        }

        txtSlovak = _pagesSlovak[_pageIndex][_sentenceIndex];
        txtForeign = _pagesForeign[_pageIndex][_sentenceIndex];
        txtForeignWord = _pagesForeignWord[_pageIndex][_sentenceIndex];

        txtSentencesCount = "/" + _pagesSlovak[_pageIndex].Count.ToString();

        Data_Loaded();
    }

    private async void Data_Loaded()
    {
        var audioIndex = _sentenceIndex + 1;
        var audioIndexStr = audioIndex.ToString();
        if (audioIndexStr.Length == 1)
        {
            audioIndexStr = "0" + audioIndexStr;
        }

        _audioPageIndex = _pageIndex + Shifter;
        selectForeignWord = (_pageIndex + 1).ToString();

        //PlayAudio(audioIndexStr);
    }

    private async void btnNext_Click()
    {
        StopIfPlaying();

        _sentenceIndex += 1;
        var length = _pagesSlovak[_pageIndex].Count;
        if (length - _sentenceIndex == 0) // get new page
        {
            _pageIndex += 1;
            _sentenceIndex = 0;
        }

        txtSentencesCount = "/" + _pagesSlovak[_pageIndex].Count.ToString();

        var audioSentenceIndex = _sentenceIndex + 1;
        var audioSentenceIndexStr = audioSentenceIndex.ToString();
        if (audioSentenceIndexStr.Length == 1)
        {
            audioSentenceIndexStr = "0" + audioSentenceIndexStr;
        }

        _audioPageIndex = _pageIndex + Shifter;

        //PlayAudio(audioSentenceIndexStr);

        txtPage = (_pageIndex + 1).ToString();
        selectForeignWord = (_pageIndex + 1).ToString();
        txtSentence = (_sentenceIndex + 1).ToString();

        txtSlovak = _pagesSlovak[_pageIndex][_sentenceIndex];
        txtForeign = _pagesForeign[_pageIndex][_sentenceIndex];
        txtForeignWord = _pagesForeignWord[_pageIndex][_sentenceIndex];

        await SecureStorage.Default.SetAsync("MyKey",
            selectBook + ";" + _pageIndex.ToString() + ";" + _sentenceIndex.ToString());
    }

    private async void btnPrevious_Click()
    {
        StopIfPlaying();

        _sentenceIndex -= 1;

        if (_sentenceIndex == -1) // get previous page
        {
            _pageIndex -= 1;
            var length = _pagesSlovak[_pageIndex].Count;
            _sentenceIndex = length - 1;
        }

        txtSentencesCount = "/" + _pagesSlovak[_pageIndex].Count.ToString();

        var audioSentenceIndex = _sentenceIndex + 1;
        var audioSentenceIndexStr = audioSentenceIndex.ToString();
        if (audioSentenceIndexStr.Length == 1)
        {
            audioSentenceIndexStr = "0" + audioSentenceIndexStr;
        }

        _audioPageIndex = _pageIndex + Shifter;

        //PlayAudio(audioSentenceIndexStr);

        txtPage = (_pageIndex + 1).ToString();
        selectForeignWord = (_pageIndex + 1).ToString();
        txtSentence = (_sentenceIndex + 1).ToString();

        txtSlovak = _pagesSlovak[_pageIndex][_sentenceIndex];
        txtForeign = _pagesForeign[_pageIndex][_sentenceIndex];
        txtForeignWord = _pagesForeignWord[_pageIndex][_sentenceIndex];

        await SecureStorage.Default.SetAsync("MyKey",
            selectBook + ";" + _pageIndex.ToString() + ";" + _sentenceIndex.ToString());
    }

    private async void PlayAudio(string audioSentenceIndexStr)
    {
        if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Dispose();
        }

        _mediaPlayer?.Dispose();
        _mediaPlayer = new MediaPlayer();

        var uri = Android.Net.Uri.Parse("file://" + LanguageFolder + "/" + _audioFolder + "/" + _audioPageIndex +
                                        "/Sentence" + audioSentenceIndexStr + ".MP3");

        _mediaPlayer.SetDataSource(Platform.CurrentActivity, uri);
        _mediaPlayer.Prepare();
        _mediaPlayer.Looping = true;
        _mediaPlayer.PlaybackParams.SetSpeed(1.0f);
        _mediaPlayer.Start();
    }

    private void StopIfPlaying()
    {
        if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
        {
            _mediaPlayer.Stop();
        }
    }

    private void btnPlayStop_Click()
    {
        if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
        {
            _mediaPlayer.Stop();
        }
        else
        {
            var audioIndex = _sentenceIndex + 1;
            var audioIndexStr = audioIndex.ToString();
            if (audioIndexStr.Length == 1)
            {
                audioIndexStr = "0" + audioIndexStr;
            }

            PlayAudio(audioIndexStr);
        }
    }

    private void PageValueChangeHandler(ChangeEventArgs e)
    {
        if (e.Value == null)
        {
            return;
        }

        var pageIndexStr = e.Value.ToString();
        txtPage = pageIndexStr;
        int.TryParse(pageIndexStr, out var pageIndex);

        var audioSentenceIndex = _sentenceIndex + 1;
        var audioSentenceIndexStr = audioSentenceIndex.ToString();
        if (audioSentenceIndexStr.Length == 1)
        {
            audioSentenceIndexStr = "0" + audioSentenceIndexStr;
        }

        pageIndex -= 1; // align with zero based indexing
        _audioPageIndex = pageIndex + Shifter;

        PlayAudio(audioSentenceIndexStr);

        txtSlovak = _pagesSlovak[pageIndex][_sentenceIndex];
        txtForeign = _pagesForeign[pageIndex][_sentenceIndex];
        txtForeignWord = _pagesForeignWord[_pageIndex][_sentenceIndex];
        _pageIndex = pageIndex;
    }

    private void BookValueChangeHandler(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            _ = int.TryParse(e.Value.ToString(), out var book);
            selectBook = book.ToString();
            LoadData(book);
        }
    }

    private void CheckboxChanged(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            disabled = (bool) e.Value;
        }
    }
}