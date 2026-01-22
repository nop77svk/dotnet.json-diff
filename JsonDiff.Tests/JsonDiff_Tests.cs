namespace NoP77svk.JsonDiff.Tests;

using System.Text.Json;
using System.Text.Json.Nodes;

using NoP77svk.JsonDiff;

[TestFixture]
public class JsonDiff_Tests
{
    public static readonly IEnumerable<string> _testCaseFileNames = Directory.EnumerateFiles(nameof(JsonDiff_Tests), "*.json", SearchOption.AllDirectories);

    private readonly JsonDocumentOptions _jsonDocumentParseOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
        AllowDuplicateProperties = true,
        MaxDepth = 20
    };

    private readonly JsonNodeOptions _jsonNodeParseOptions = new()
    {
        PropertyNameCaseInsensitive = false
    };

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task Basic_JsonElement_SelfComparison_WorksOK(string testCaseFileName)
    {
        // arrange
        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonDocument.CompareWith(jsonDocument);

        // assert
        Assert.That(differences, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task Basic_JsonNode_SelfComparison_WorksOK(string testCaseFileName)
    {
        // arrange
        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        Assert.That(jsonDocument, Is.Not.Null);

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonDocument?.CompareWith(jsonDocument)
            ?? throw new InvalidOperationException("JsonNode parsing resulted in null");

        // assert
        Assert.That(differences, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task JsonComparerInstance_JsonElement_SelfComparison_WorksOK(string testCaseFileName)
    {
        // arrange
        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);
        JsonElementComparer jsonComparer = new ();

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(jsonDocument.RootElement, jsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task JsonComparerInstance_JsonNode_SelfComparison_WorksOK(string testCaseFileName)
    {
        // arrange
        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        JsonNodeComparer jsonComparer = new ();

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(jsonDocument, jsonDocument);

        // assert
        Assert.That(differences, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task JsonComparerInstance_JsonElement_SelfComparisonByPosition_WorksOK(string testCaseFileName)
    {
        // arrange
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
        Assert.That(differences, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task JsonComparerInstance_JsonNode_SelfComparisonByPosition_WorksOK(string testCaseFileName)
    {
        // arrange
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
        Assert.That(differences, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task JsonComparerInstance_JsonElement_SelfComparisonByKeyAndPosition_WorksOK(string testCaseFileName)
    {
        // arrange
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
        Assert.That(differences, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task JsonComparerInstance_JsonNode_SelfComparisonByKeyAndPosition_WorksOK(string testCaseFileName)
    {
        // arrange
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
        Assert.That(differences, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task JsonComparerInstance_JsonElement_SelfComparisonByPositionAndName_WorksOK(string testCaseFileName)
    {
        // arrange
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
        Assert.That(differences, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task JsonComparerInstance_JsonNode_SelfComparisonByPositionAndName_WorksOK(string testCaseFileName)
    {
        // arrange
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
        Assert.That(differences, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task JsonComparerInstance_JsonElement_SelfComparisonByKeyAndName_WorksOK(string testCaseFileName)
    {
        // arrange
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
        Assert.That(differences, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(_testCaseFileNames))]
    public async Task JsonComparerInstance_JsonNode_SelfComparisonByKeyAndName_WorksOK(string testCaseFileName)
    {
        // arrange
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
        Assert.That(differences, Is.Empty);
    }
}
