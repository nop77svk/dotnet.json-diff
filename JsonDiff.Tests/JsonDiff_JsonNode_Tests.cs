#if NET8_0_OR_GREATER
namespace NoP77svk.JsonDiff.Tests;

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using NoP77svk.JsonDiff;

public class JsonDiff_JsonNode_Tests
{
    private readonly SimpleJsonDiffFormatter<JsonNode?> _jsonNodeDiffFormatter = new();

    private readonly JsonDocumentOptions _jsonDocumentParseOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
#if NET10_0_OR_GREATER
        AllowDuplicateProperties = true,
#endif
        MaxDepth = 20
    };

    private readonly JsonNodeOptions _jsonNodeParseOptions = new()
    {
        PropertyNameCaseInsensitive = false
    };

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
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

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
    public async Task JsonNode_ShuffledKvpComparisonByKeyAndName_ReturnsEmpty(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        JsonNode? originalJsonDocument = await JsonNode.ParseAsync(originalJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        JsonNode? shuffledJsonDocument = await JsonNode.ParseAsync(shuffledJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        IJsonDiffNodeValuesSelector<JsonNode?> kvpMatchingNodeValuesSelector = new JsonNodeDiffValuesSelector()
        {
            ArrayElementDescriptorSelector = (index, elementNode) =>
            {
                if (elementNode is not JsonObject element)
                {
                    return $"element #{index}";
                }

                string? result = null;
                if (element.TryGetPropertyValue("key", out JsonNode? keyProperty)
                    || element.TryGetPropertyValue("id", out keyProperty)
                    || element.TryGetPropertyValue("type", out keyProperty)
                    || element.TryGetPropertyValue("org_id", out keyProperty)
                    || element.TryGetPropertyValue("user_id", out keyProperty)
                    || element.TryGetPropertyValue("widget_id", out keyProperty))
                {
                    result = keyProperty?.GetValueKind() is JsonValueKind.String or JsonValueKind.Number
                        ? keyProperty.GetValue<string>()
                        : null;
                }

                return result ?? $"element #{index}";
            }
        };

        JsonComparer<JsonNode?> jsonComparer = new(kvpMatchingNodeValuesSelector)
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
    public async Task JsonNode_ShuffledKvpComparisonByPositionAndName_ReturnsDifferences(ShuffledJsonTestCase testCase)
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
        Assert.That(differences, Is.Not.Empty);
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
#endif
