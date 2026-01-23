namespace NoP77svk.JsonDiff.Tests;

using System.Text;
using System.Text.Json;

using NoP77svk.JsonDiff;

[TestFixture]
public class JsonDiff_JsonElement_Tests
{
    private readonly JsonDocumentOptions _jsonDocumentParseOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
#if NET10_0_OR_GREATER
        AllowDuplicateProperties = true,
#endif
        MaxDepth = 20
    };

    private readonly SimpleJsonDiffFormatter<JsonElement> _jsonElementDiffFormatter = new();

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
    public async Task JsonElement_ShuffledKvpComparisonByKeyAndName_ReturnsEmpty(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        using JsonDocument originalJsonDocument = await JsonDocument.ParseAsync(originalJsonStream, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        using JsonDocument shuffledJsonDocument = await JsonDocument.ParseAsync(shuffledJsonStream, _jsonDocumentParseOptions);

        IJsonDiffNodeValuesSelector<JsonElement> kvpMatchingNodeValuesSelector = new JsonElementDiffValuesSelector()
        {
            ArrayElementDescriptorSelector = (index, element) =>
            {
                if (element.ValueKind is not JsonValueKind.Object)
                {
                    return $"element #{index}";
                }

                string? result = null;
                if (element.TryGetProperty("key", out JsonElement keyProperty)
                    || element.TryGetProperty("id", out keyProperty)
                    || element.TryGetProperty("type", out keyProperty)
                    || element.TryGetProperty("org_id", out keyProperty)
                    || element.TryGetProperty("user_id", out keyProperty)
                    || element.TryGetProperty("widget_id", out keyProperty))
                {
                    result = keyProperty.ValueKind is JsonValueKind.String or JsonValueKind.Number
                        ? keyProperty.GetRawText()
                        : null;
                }

                return result ?? $"element #{index}";
            }
        };

        JsonComparer<JsonElement> jsonComparer = new(kvpMatchingNodeValuesSelector)
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
    public async Task JsonElement_ShuffledKvpComparisonByPositionAndName_ReturnsDifferences(ShuffledJsonTestCase testCase)
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
}
