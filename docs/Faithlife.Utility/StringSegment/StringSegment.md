# StringSegment constructor (1 of 4)

Initializes a new instance of the [`StringSegment`](../StringSegment.md) class.

```csharp
public StringSegment(string? source)
```

| parameter | description |
| --- | --- |
| source | The source string. |

## Remarks

Creates a segment that represents the entire source string.

## See Also

* struct [StringSegment](../StringSegment.md)
* namespace [Faithlife.Utility](../../Faithlife.Utility.md)

---

# StringSegment constructor (2 of 4)

Initializes a new instance of the [`StringSegment`](../StringSegment.md) class.

```csharp
public StringSegment(string source, Capture capture)
```

| parameter | description |
| --- | --- |
| source | The source string. |
| capture | The Capture to represent. |

## See Also

* struct [StringSegment](../StringSegment.md)
* namespace [Faithlife.Utility](../../Faithlife.Utility.md)

---

# StringSegment constructor (3 of 4)

Initializes a new instance of the [`StringSegment`](../StringSegment.md) class.

```csharp
public StringSegment(string? source, int offset)
```

| parameter | description |
| --- | --- |
| source | The source string. |
| offset | The offset of the segment. |

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | The offset is out of range. |

## Remarks

Creates a segment that starts at the specified offset and continues to the end of the source string.

## See Also

* struct [StringSegment](../StringSegment.md)
* namespace [Faithlife.Utility](../../Faithlife.Utility.md)

---

# StringSegment constructor (4 of 4)

Initializes a new instance of the [`StringSegment`](../StringSegment.md) class.

```csharp
public StringSegment(string? source, int offset, int length)
```

| parameter | description |
| --- | --- |
| source | The source string. |
| offset | The offset of the segment. |
| length | The length of the segment. |

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | The offset or length are out of range. |

## See Also

* struct [StringSegment](../StringSegment.md)
* namespace [Faithlife.Utility](../../Faithlife.Utility.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->
