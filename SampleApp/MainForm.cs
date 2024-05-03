using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrulyRandom;
using TrulyRandom.Modules;
using TrulyRandom.Modules.Extractors;
using TrulyRandom.Modules.Sources;

#pragma warning disable IDE1006 // Naming Styles
namespace SampleApp;

public partial class MainForm : Form
{
    //Modules
    private VideoSource videoSource;
    private AudioSource audioSource;
    private BiologicalSource biologicalSource;
    private DeflateExtractor deflateExtractor1;
    private DeflateExtractor deflateExtractor2;
    private ShuffleExtractor shuffleExtractor;
    private HashExtractor hashExtractor;
    private Tester tester;
    private TrulyRandom.Modules.Buffer buffer;

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        type.SelectedIndex = 0;
        new Task(() =>
        {
            //Creating modules
            biologicalSource = new BiologicalSource();
            deflateExtractor1 = new DeflateExtractor();
            deflateExtractor2 = new DeflateExtractor();
            shuffleExtractor = new ShuffleExtractor();
            hashExtractor = new HashExtractor();
            tester = new Tester();
            buffer = new TrulyRandom.Modules.Buffer();

            //Linking modules together
            shuffleExtractor.AddSource(deflateExtractor1);
            shuffleExtractor.AddSource(deflateExtractor2);
            shuffleExtractor.AddSource(biologicalSource);
            hashExtractor.AddSource(shuffleExtractor);
            tester.AddSource(hashExtractor);
            buffer.AddSource(tester);

            //Adding seed sources (optional)
            shuffleExtractor.SeedSource = buffer;
            hashExtractor.SeedSource = buffer;

            //Adding dynamic coefficients, now these extractors will compress more if buffer is close to be full
            deflateExtractor1.DynamicCoefficientSource = buffer;
            deflateExtractor2.DynamicCoefficientSource = buffer;
            shuffleExtractor.DynamicCoefficientSource = buffer;
            hashExtractor.DynamicCoefficientSource = buffer;

            //Manual test selection
            tester.AutoSelectTests = false;
            tester.TestParameters.TestsToPerform = ReadTests();

            //biologicalSource.Start();
            //deflateExtractor1.Start();
            //deflateExtractor2.Start();
            //shuffleExtractor.Start();
            //hashExtractor.Start();
            //tester.Start();
            buffer.Start();

            //Searching for available sources
            BeginInvoke(() => { videoStub.Text = "Searching for device..."; });
            try
            {
                videoSource = new VideoSource();
                BeginInvoke(() =>
                {
                    videoStub.Visible = false;
                    videoName.Text = videoSource.DeviceName;
                });
            }
            catch (DeviceNotFoundException)
            {
                BeginInvoke(() => { videoStub.Text = "No operational devices found"; });
            }


            BeginInvoke(() => { audioStub.Text = "Searching for device..."; });
            try
            {
                audioSource = new AudioSource();
                BeginInvoke(() =>
                {
                    audioStub.Visible = false;
                    audioName.Text = audioSource.DeviceName;
                });
            }
            catch (DeviceNotFoundException)
            {
                BeginInvoke(() => { audioStub.Text = "No operational devices found"; });
            }

            if (videoSource != null)
            {
                //videoSource.Start();
                deflateExtractor1.AddSource(videoSource);
            }
            if (audioSource != null)
            {
                //audioSource.Start();
                deflateExtractor2.AddSource(audioSource);
            }

            BeginInvoke(() => { refreshTimer.Start(); });
        }).Start();
    }

    /// <summary>
    /// Reads enabled tests from checkboxes
    /// </summary>
    private NistTests.TestType ReadTests()
    {
        NistTests.TestType result = NistTests.TestType.None;

        if (checkBoxFrequency.Checked)
        {
            result |= NistTests.TestType.Frequency;
        }
        if (checkBoxBlockFrequency.Checked)
        {
            result |= NistTests.TestType.BlockFrequency;
        }
        if (checkBoxRuns.Checked)
        {
            result |= NistTests.TestType.Runs;
        }
        if (checkBoxLongestRunOfOnes.Checked)
        {
            result |= NistTests.TestType.LongestRunOfOnes;
        }
        if (checkBoxBinaryMatrixRank.Checked)
        {
            result |= NistTests.TestType.BinaryMatrixRank;
        }
        if (checkBoxDft.Checked)
        {
            result |= NistTests.TestType.DiscreteFourierTransform;
        }
        if (checkBoxNonOverlappingTemplateMatchings.Checked)
        {
            result |= NistTests.TestType.NonOverlappingTemplateMatchings;
        }
        if (checkBoxOverlappingTemplateMatchings.Checked)
        {
            result |= NistTests.TestType.OverlappingTemplateMatchings;
        }
        if (checkBoxMaurersUniversal.Checked)
        {
            result |= NistTests.TestType.MaurersUniversal;
        }
        if (checkBoxLinearComplexity.Checked)
        {
            result |= NistTests.TestType.LinearComplexity;
        }
        if (checkBoxSerial.Checked)
        {
            result |= NistTests.TestType.Serial;
        }
        if (checkBoxApproximateEntropy.Checked)
        {
            result |= NistTests.TestType.ApproximateEntropy;
        }
        if (checkBoxCumulativeSums.Checked)
        {
            result |= NistTests.TestType.CumulativeSums;
        }
        if (checkBoxRandomExcursions.Checked)
        {
            result |= NistTests.TestType.RandomExcursions;
        }
        if (checkBoxRandomExcursionsVariant.Checked)
        {
            result |= NistTests.TestType.RandomExcursionsVariant;
        }

        return result;
    }

    private void refreshTimer_Tick(object sender, EventArgs e)
    {
        //Refreshing data display
        if (videoSource != null)
        {
            videoTotal.Text = $"Total bytes: {Utils.FormatBytes(videoSource.TotalBytesGenerated)}";
            videoBps.Text = $"Bytes per second: {Utils.FormatBytes(videoSource.BytesPerSecond)}";
            videoCurrent.Text = $"{Utils.FormatBytes(videoSource.BytesInBuffer)}";
            videoCurrentBar.Value = (int)(videoSource.BufferState * 100);
        }

        if (audioSource != null)
        {
            audioTotal.Text = $"Total bytes: {Utils.FormatBytes(audioSource.TotalBytesGenerated)}";
            audioBps.Text = $"Bytes per second: {Utils.FormatBytes(audioSource.BytesPerSecond)}";
            audioCurrent.Text = $"{Utils.FormatBytes(audioSource.BytesInBuffer)}";
            audioCurrentBar.Value = (int)(audioSource.BufferState * 100);
        }

        biologicalTotal.Text = $"Total bytes: {Utils.FormatBytes(biologicalSource.TotalBytesGenerated)}";
        biologicalBps.Text = $"Bytes per second: {Utils.FormatBytes(biologicalSource.BytesPerSecond)}";
        biologicalCurrent.Text = $"{Utils.FormatBytes(biologicalSource.BytesInBuffer)}";
        biologicalCurrentBar.Value = (int)(biologicalSource.BufferState * 100);

        deflate1Total.Text = $"Total bytes: {Utils.FormatBytes(deflateExtractor1.TotalBytesGenerated)}";
        deflate1Bps.Text = $"Bytes per second: {Utils.FormatBytes(deflateExtractor1.BytesPerSecond)}";
        deflate1Current.Text = $"{Utils.FormatBytes(deflateExtractor1.BytesInBuffer)}";
        deflate1Compression.Text = $"Compression ratio: {deflateExtractor1.ActualCompression:F2}";
        deflate1CurrentBar.Value = (int)(deflateExtractor1.BufferState * 100);

        deflate2Total.Text = $"Total bytes: {Utils.FormatBytes(deflateExtractor2.TotalBytesGenerated)}";
        deflate2Bps.Text = $"Bytes per second: {Utils.FormatBytes(deflateExtractor2.BytesPerSecond)}";
        deflate2Current.Text = $"{Utils.FormatBytes(deflateExtractor2.BytesInBuffer)}";
        deflate2Compression.Text = $"Compression ratio: {deflateExtractor2.ActualCompression:F2}";
        deflate2CurrentBar.Value = (int)(deflateExtractor2.BufferState * 100);

        shuffleTotal.Text = $"Total bytes: {Utils.FormatBytes(shuffleExtractor.TotalBytesGenerated)}";
        shuffleBps.Text = $"Bytes per second: {Utils.FormatBytes(shuffleExtractor.BytesPerSecond)}";
        shuffleCurrent.Text = $"{Utils.FormatBytes(shuffleExtractor.BytesInBuffer)}";
        shuffleCompression.Text = $"Compression ratio: {shuffleExtractor.ActualCompression:F2}";
        shuffleCurrentBar.Value = (int)(shuffleExtractor.BufferState * 100);

        hashTotal.Text = $"Total bytes: {Utils.FormatBytes(hashExtractor.TotalBytesGenerated)}";
        hashBps.Text = $"Bytes per second: {Utils.FormatBytes(hashExtractor.BytesPerSecond)}";
        hashCurrent.Text = $"{Utils.FormatBytes(hashExtractor.BytesInBuffer)}";
        hashCompression.Text = $"Compression ratio: {hashExtractor.ActualCompression:F2}";
        hashCurrentBar.Value = (int)(hashExtractor.BufferState * 100);

        testerTotal.Text = $"Total bytes: {Utils.FormatBytes(tester.TotalBytesGenerated)}";
        testerBps.Text = $"Bytes per second: {Utils.FormatBytes(tester.BytesPerSecond)}";
        testerCurrent.Text = $"{Utils.FormatBytes(tester.BytesInBuffer)}";
        testerSuccessRate.Text = $"Success rate: {tester.SuccessRate:F2}";
        testerCurrentBar.Value = (int)(tester.BufferState * 100);

        bufferCurrent.Text = $"{Utils.FormatBytes(buffer.BytesInBuffer)}";
        bufferCurrentBar.Value = (int)(buffer.BufferState * 100);

        if (videoSource != null)
        {
            videoPause.Text = videoSource.Started ? "Pause" : "Start";
        }
        if (audioSource != null)
        {
            audioPause.Text = audioSource.Started ? "Pause" : "Start";
        }
        biologicalPause.Text = biologicalSource.Started ? "Pause" : "Start";
        deflate1Pause.Text = deflateExtractor1.Started ? "Pause" : "Start";
        deflate2Pause.Text = deflateExtractor2.Started ? "Pause" : "Start";
        shufflePause.Text = shuffleExtractor.Started ? "Pause" : "Start";
        hashPause.Text = hashExtractor.Started ? "Pause" : "Start";
        testerPause.Text = tester.Started ? "Pause" : "Start";

        if (tester.LastTestResult != null && lastTestReport.Text != tester.LastTestResult.Report)
        {
            lastTestReport.Text = tester.LastTestResult.Report;

            lastResultFrequency.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.Frequency) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.Frequency].Success ? "Success" : "Failure");

            lastResultBlockFrequency.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.BlockFrequency) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.BlockFrequency].Success ? "Success" : "Failure");

            lastResultRuns.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.Runs) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.Runs].Success ? "Success" : "Failure");

            lastResultLongestRunOfOnes.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.LongestRunOfOnes) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.LongestRunOfOnes].Success ? "Success" : "Failure");

            lastResultBinaryMatrixRank.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.BinaryMatrixRank) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.BinaryMatrixRank].Success ? "Success" : "Failure");

            lastResultDft.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.DiscreteFourierTransform) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.DiscreteFourierTransform].Success ? "Success" : "Failure");

            lastResultNonOverlappingTemplateMatchings.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.NonOverlappingTemplateMatchings) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.NonOverlappingTemplateMatchings].Success ? "Success" : "Failure");

            lastResultOverlappingTemplateMatchings.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.OverlappingTemplateMatchings) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.OverlappingTemplateMatchings].Success ? "Success" : "Failure");

            lastResultMaurersUniversal.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.MaurersUniversal) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.MaurersUniversal].Success ? "Success" : "Failure");

            lastResultLinearComplexity.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.LinearComplexity) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.LinearComplexity].Success ? "Success" : "Failure");

            lastResultSerial.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.Serial) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.Serial].Success ? "Success" : "Failure");

            lastResultApproximateEntropy.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.ApproximateEntropy) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.ApproximateEntropy].Success ? "Success" : "Failure");

            lastResultCumulativeSums.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.CumulativeSums) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.CumulativeSums].Success ? "Success" : "Failure");

            lastResultRandomExcursions.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.RandomExcursions) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.RandomExcursions].Success ? "Success" : "Failure");

            lastResultRandomExcursionsVariant.Text = !tester.LastTestResult.TestResults.ContainsKey(NistTests.TestType.RandomExcursionsVariant) ? "Not performed" :
                (tester.LastTestResult.TestResults[NistTests.TestType.RandomExcursionsVariant].Success ? "Success" : "Failure");
        }
    }

    private void videoPause_Click(object sender, EventArgs e)
    {
        if (videoSource != null)
        {
            videoSource.Started = !videoSource.Started;
        }
    }

    private void audioPause_Click(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            audioSource.Started = !audioSource.Started;
        }
    }

    private void biologicalPause_Click(object sender, EventArgs e)
    {
        biologicalSource.Started = !biologicalSource.Started;
    }

    private void deflate1Pause_Click(object sender, EventArgs e)
    {
        deflateExtractor1.Started = !deflateExtractor1.Started;
    }

    private void deflate2Pause_Click(object sender, EventArgs e)
    {
        deflateExtractor2.Started = !deflateExtractor2.Started;
    }

    private void shufflePause_Click(object sender, EventArgs e)
    {
        shuffleExtractor.Started = !shuffleExtractor.Started;
    }

    private void hashPause_Click(object sender, EventArgs e)
    {
        hashExtractor.Started = !hashExtractor.Started;
    }

    private void testerPause_Click(object sender, EventArgs e)
    {
        tester.Started = !tester.Started;
    }

    private void testCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        tester.TestParameters.TestsToPerform = ReadTests();
    }

    private void type_SelectedIndexChanged(object sender, EventArgs e)
    {
        //bit and byte types do not support bounds
        if (type.SelectedIndex == 0 || type.SelectedIndex == 1)
        {
            lowerBound.Enabled = false;
            upperBound.Enabled = false;
        }
        else
        {
            lowerBound.Enabled = true;
            upperBound.Enabled = true;
        }
    }

    private void generate_Click(object sender, EventArgs e)
    {
        try
        {
            switch (type.SelectedIndex)
            {
                case 0:
                    bool[] result0 = new bool[int.Parse(count.Text)];
                    for (int i = 0; i < int.Parse(count.Text); i++)
                    {
                        result0[i] = buffer.DataSource.GetBit();
                    }
                    results.Text = string.Join("\n", result0.Select(x => x ? 1 : 0));
                    break;
                case 1:
                    byte[] result1 = buffer.DataSource.GetBytes(int.Parse(count.Text));
                    results.Text = string.Join("\n", result1.Select(x => x.ToString()));
                    break;
                case 2:
                    int upperInt = int.Parse(upperBound.Text);
                    int lowerInt = int.Parse(lowerBound.Text);
                    int[] result2 = new int[int.Parse(count.Text)];

                    for (int i = 0; i < int.Parse(count.Text); i++)
                    {
                        result2[i] = buffer.DataSource.GetInt(lowerInt, upperInt);
                    }
                    results.Text = string.Join("\n", result2.Select(x => x.ToString()));
                    break;
                case 3:
                    uint upperUInt = uint.Parse(upperBound.Text);
                    uint lowerUInt = uint.Parse(lowerBound.Text);
                    uint[] result3 = new uint[int.Parse(count.Text)];

                    for (int i = 0; i < int.Parse(count.Text); i++)
                    {
                        result3[i] = buffer.DataSource.GetUInt(lowerUInt, upperUInt);
                    }
                    results.Text = string.Join("\n", result3.Select(x => x.ToString()));
                    break;
                case 4:
                    long upperLong = long.Parse(upperBound.Text);
                    long lowerLong = long.Parse(lowerBound.Text);
                    long[] result4 = new long[int.Parse(count.Text)];

                    for (int i = 0; i < int.Parse(count.Text); i++)
                    {
                        result4[i] = buffer.DataSource.GetLong(lowerLong, upperLong);
                    }
                    results.Text = string.Join("\n", result4.Select(x => x.ToString()));
                    break;
                case 5:
                    ulong upperULong = ulong.Parse(upperBound.Text);
                    ulong lowerULong = ulong.Parse(lowerBound.Text);
                    ulong[] result5 = new ulong[int.Parse(count.Text)];

                    for (int i = 0; i < int.Parse(count.Text); i++)
                    {
                        result5[i] = buffer.DataSource.GetULong(lowerULong, upperULong);
                    }
                    results.Text = string.Join("\n", result5.Select(x => x.ToString()));
                    break;
            }
        }
        catch (Exception e1)
        {
            results.Text = e1.Message;
        }
    }

    private void bufferClear_Click(object sender, EventArgs e)
    {
        buffer.ClearBuffer();
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        refreshTimer.Stop();
        biologicalSource.Dispose();
        deflateExtractor1.Dispose();
        deflateExtractor2.Dispose();
        shuffleExtractor.Dispose();
        hashExtractor.Dispose();
        tester.Dispose();
        buffer.Dispose();
    }
}
