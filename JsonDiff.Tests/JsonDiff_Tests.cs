namespace NoP77svk.JsonDiff.Tests;

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

using NoP77svk.JsonDiff;

[TestFixture]
public class JsonDiff_Tests
{
    private const string ShuffledFileSuffix = "shuffled";
    private const string ShuffledKvpFileSuffix = "shuffled-kvp";

    public static IEnumerable<string> BasicTestCases => Directory.EnumerateFiles(nameof(JsonDiff_Tests), "*.json", SearchOption.AllDirectories)
        .Where(fname => !fname.EndsWith($".{ShuffledFileSuffix}.json", StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<ShuffledJsonTestCase> ShuffledTestCases => Directory.EnumerateFiles(nameof(JsonDiff_Tests), $"*.{ShuffledFileSuffix}.json", SearchOption.AllDirectories)
        .Select(fname => new ShuffledJsonTestCase(_rxRemoveShuffledSuffix.Replace(fname, ".json"), fname));

    public static IEnumerable<ShuffledJsonTestCase> ShuffledKvpTestCases => Directory.EnumerateFiles(nameof(JsonDiff_Tests), $"*.{ShuffledKvpFileSuffix}.json", SearchOption.AllDirectories)
        .Select(fname => new ShuffledJsonTestCase(_rxRemoveShuffledKvpSuffix.Replace(fname, ".json"), fname));

    private static readonly Regex _rxRemoveShuffledSuffix = new Regex(@$"\.{ShuffledFileSuffix}\.json$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex _rxRemoveShuffledKvpSuffix = new Regex(@$"\.{ShuffledKvpFileSuffix}\.json$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

#pragma warning disable SA1313
    public record ShuffledJsonTestCase(string OriginalFileName, string ShuffledFileName);
#pragma warning restore SA1313

    private readonly JsonDocumentOptions _jsonDocumentParseOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
        AllowDuplicateProperties = true,
        MaxDepth = 20
    };

    private readonly SimpleJsonDiffFormatter<JsonElement> _jsonElementDiffFormatter = new();
    private readonly SimpleJsonDiffFormatter<JsonNode?> _jsonNodeDiffFormatter = new();

    private readonly JsonNodeOptions _jsonNodeParseOptions = new()
    {
        PropertyNameCaseInsensitive = false
    };

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task Basic_JsonElement_SelfComparison_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonDocument.CompareWith(jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task Basic_JsonNode_SelfComparison_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        Assert.That(jsonDocument, Is.Not.Null);

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonDocument?.CompareWith(jsonDocument)
            ?? throw new InvalidOperationException("JsonNode parsing resulted in null");

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task JsonElement_SelfComparison_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);
        JsonElementComparer jsonComparer = new();

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(jsonDocument.RootElement, jsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task JsonNode_SelfComparison_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        JsonNodeComparer jsonComparer = new();

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(jsonDocument, jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task JsonElement_SelfComparisonByPosition_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);
        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(jsonDocument.RootElement, jsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task JsonNode_SelfComparisonByPosition_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(jsonDocument, jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task JsonElement_SelfComparisonByKeyAndPosition_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);
        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(jsonDocument.RootElement, jsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task JsonNode_SelfComparisonByKeyAndPosition_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(jsonDocument, jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task JsonElement_SelfComparisonByPositionAndName_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);
        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(jsonDocument.RootElement, jsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task JsonNode_SelfComparisonByPositionAndName_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(jsonDocument, jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task JsonElement_SelfComparisonByKeyAndName_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);
        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(jsonDocument.RootElement, jsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(BasicTestCases))]
    public async Task JsonNode_SelfComparisonByKeyAndName_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(jsonDocument, jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(ShuffledTestCases))]
    public async Task JsonElement_ShuffledComparisonByKeyAndName_ReturnsEmpty(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        using JsonDocument originalJsonDocument = await JsonDocument.ParseAsync(originalJsonStream, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        using JsonDocument shuffledJsonDocument = await JsonDocument.ParseAsync(shuffledJsonStream, _jsonDocumentParseOptions);

        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument.RootElement, shuffledJsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(ShuffledTestCases))]
    public async Task JsonNode_ShuffledComparisonByKeyAndName_ReturnsEmpty(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        JsonNode? originalJsonDocument = await JsonNode.ParseAsync(originalJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        JsonNode? shuffledJsonDocument = await JsonNode.ParseAsync(shuffledJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument, shuffledJsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(ShuffledTestCases))]
    public async Task JsonElement_ShuffledComparisonByPositionAndName_ReturnsEmpty(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        using JsonDocument originalJsonDocument = await JsonDocument.ParseAsync(originalJsonStream, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        using JsonDocument shuffledJsonDocument = await JsonDocument.ParseAsync(shuffledJsonStream, _jsonDocumentParseOptions);

        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument.RootElement, shuffledJsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(ShuffledTestCases))]
    public async Task JsonNode_ShuffledComparisonByPositionAndName_ReturnsEmpty(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        JsonNode? originalJsonDocument = await JsonNode.ParseAsync(originalJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        JsonNode? shuffledJsonDocument = await JsonNode.ParseAsync(shuffledJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument, shuffledJsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => DifferencesToString(differences));
    }

    [Test]
    [TestCaseSource(nameof(ShuffledTestCases))]
    public async Task JsonElement_ShuffledComparisonByKeyAndPosition_ReturnsDifferences(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        using JsonDocument originalJsonDocument = await JsonDocument.ParseAsync(originalJsonStream, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        using JsonDocument shuffledJsonDocument = await JsonDocument.ParseAsync(shuffledJsonStream, _jsonDocumentParseOptions);

        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument.RootElement, shuffledJsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Not.Empty);
    }

    [Test]
    [TestCaseSource(nameof(ShuffledTestCases))]
    public async Task JsonNode_ShuffledComparisonByKeyAndPosition_ReturnsDifferences(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        JsonNode? originalJsonDocument = await JsonNode.ParseAsync(originalJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        JsonNode? shuffledJsonDocument = await JsonNode.ParseAsync(shuffledJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument, shuffledJsonDocument);

        // assert
        Assert.That(differences, Is.Not.Empty);
    }

    [Test]
    [TestCaseSource(nameof(ShuffledTestCases))]
    public async Task JsonElement_ShuffledComparisonByPositionAndPosition_ReturnsDifferences(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        using JsonDocument originalJsonDocument = await JsonDocument.ParseAsync(originalJsonStream, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        using JsonDocument shuffledJsonDocument = await JsonDocument.ParseAsync(shuffledJsonStream, _jsonDocumentParseOptions);

        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument.RootElement, shuffledJsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Not.Empty);
    }

    [Test]
    [TestCaseSource(nameof(ShuffledTestCases))]
    public async Task JsonNode_ShuffledComparisonByPositionAndPosition_ReturnsDifferences(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        JsonNode? originalJsonDocument = await JsonNode.ParseAsync(originalJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        JsonNode? shuffledJsonDocument = await JsonNode.ParseAsync(shuffledJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument, shuffledJsonDocument);

        // assert
        Assert.That(differences, Is.Not.Empty);
    }

    private string DifferencesToString(IEnumerable<JsonDifference<JsonElement>> differences)
    {
        StringBuilder result = new();
        result.AppendLine("Differences:");
        result.AppendLine("------------------------------------------------------------------------------");

        foreach (string diff in differences.AsFormattedStrings(_jsonElementDiffFormatter))
        {
            result.AppendLine(diff);
        }

        result.AppendLine("------------------------------------------------------------------------------");

        return result.ToString();
    }

    private string DifferencesToString(IEnumerable<JsonDifference<JsonNode?>> differences)
    {
        StringBuilder result = new();
        result.AppendLine("Differences:");
        result.AppendLine("------------------------------------------------------------------------------");

        foreach (string diff in differences.AsFormattedStrings(_jsonNodeDiffFormatter))
        {
            result.AppendLine(diff);
        }

        result.AppendLine("------------------------------------------------------------------------------");

        return result.ToString();
    }
}
