---
name: kmy-analyze
description: Static analysis and code quality checks for KMyMoney.Net
allowed_models: all
---

# KMyMoney.Net Analysis Skill

## Purpose
Perform static analysis, code quality checks, and enforce project rules from AGENTS.md.

## Activation
Load this skill when you need to:
- Check code for AGENTS.md violations
- Analyze code quality and patterns
- Find dead code or unused dependencies
- Verify naming conventions
- Check for architectural issues

## Commands

### AGENTS.md Rule Enforcement

#### Rule 1: One Class Per File
```bash
# Find files with multiple classes (violation)
find . -name "*.cs" -type f | while read file; do
    class_count=$(grep -c "^\s*public\s*class\s\+\|^\s*public\s*interface\s\+\|^\s*public\s*enum\s\+\|^\s*public\s*struct\s\+" "$file" 2>/dev/null || echo 0)
    if [ "$class_count" -gt 1 ]; then
        echo "VIOLATION: $file has $class_count classes"
    fi
done

# Find files with multiple non-nested classes
# (This handles partial classes and nested classes)
grep -r "^[[:space:]]*public\s\+(class|interface|enum|struct)\s\+" . --include="*.cs" | \
    grep -v "partial" | \
    awk -F: '{print $1}' | \
    sort | \
    uniq -c | \
    awk '$1 > 1 {print "VIOLATION: " $2 " has multiple top-level types"}'
```

#### Rule 2: TDD - Tests Should Exist
```bash
# Find production classes without tests
# Get all class names from Core project
classes=$(grep -rh "^[[:space:]]*public\s\+class\s\+" KMyMoney.Net.Core --include="*.cs" | \
    sed -E 's/.*public\s+class\s+([A-Za-z0-9_]+).*/\1/')

# Get all test classes
test_classes=$(grep -rh "^[[:space:]]*public\s\+class\s\+" KMyMoney.Net.Core.Tests --include="*.cs" | \
    sed -E 's/.*public\s+class\s+([A-Za-z0-9_]+).*/\1/' | \
    sed 's/Tests$//')

# Find classes without tests
comm -23 <(echo "$classes" | sort | uniq) <(echo "$test_classes" | sort | uniq)
```

#### Rule 3: File Per Class Validation
```bash
# List all files and their class count
find . -name "*.cs" -type f | while read file; do
    classes=$(grep -c "^[[:space:]]*public\s\+class\s\+" "$file" 2>/dev/null || echo 0)
    echo "$file: $classes classes"
done | grep -v ": 0 classes" | sort -k2 -rn
```

### Code Quality Analysis

#### Complexity Analysis
```bash
# Install dotnet-cyclowalk (cyclomatic complexity)
dotnet tool install --global dotnet-cyclowalk

# Analyze project
dotnet cyclowalk KMyMoney.Net.Core/KMyMoney.Net.Core.csproj
```

#### Code Duplication
```bash
# Use dotnet-format and similar tools
# Or manual check with grep

# Find duplicate method signatures
grep -rh "^[[:space:]]*public\s\+.*(" . --include="*.cs" | sort | uniq -c | grep -v "1 "
```

#### Unused Code Detection
```bash
# Find private methods never called (requires more sophisticated tooling)
# Use Visual Studio's CodeLens or ReSharper CLI

# Simple check: find methods with no internal calls
grep -rh "private.*(" . --include="*.cs" | head -20
```

### Dependency Analysis

#### Find Unused Dependencies
```bash
dotnet list package --unused

# For each project
find . -name "*.csproj" -type f | while read proj; do
    echo "=== $proj ==="
    dotnet list "$proj" package --unused
done
```

#### Find All Dependencies
```bash
# Full dependency tree
dotnet list KMyMoney.Net.TelegramBot package --include-transitive --format json
```

### Naming Convention Checks

#### PascalCase for Classes/Interfaces
```bash
# Find non-PascalCase class names
grep -rh "^[[:space:]]*public\s\+(class|interface)\s\+" . --include="*.cs" | \
grep -vE "^[[:space:]]*public\s\+(class|interface)\s\+[A-Z][a-zA-Z0-9]" | \
echo "Potential naming violations found"
```

#### Interface Prefix Check (I)
```bash
# Find interfaces not starting with I
grep -rh "^[[:space:]]*public\s\+interface\s\+" . --include="*.cs" | \
grep -vE "^[[:space:]]*public\s\+interface\s\+I[A-Z]" | \
echo "Interfaces should start with I:"
```

### Architecture Analysis

#### Project Reference Graph
```bash
# Show project dependencies
dotnet list KMyMoney.Net.sln project

# Detailed reference graph
dotnet list KMyMoney.Net.TelegramBot reference
```

#### Circular Dependency Detection
```bash
# Check for circular references
# Manual: trace reference chain
# Or use architectural tools

# Simple check: if A references B and B references A
dotnet list KMyMoney.Net.Core reference | grep TelegramBot
dotnet list KMyMoney.Net.TelegramBot reference | grep Core
```

### Test Coverage Analysis

#### Run Coverage Report
```bash
# Install coverlet
dotnet add KMyMoney.Net.Core.Tests package coverlet.collector

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

#### Coverlet runsettings
```xml
<!-- coverlet.runsettings -->
<RunSettings>
    <DataCollectionRunSettings>
        <DataCollectors>
            <DataCollector friendlyName="XPlat code coverage">
                <Configuration>
                    <Format>cobertura</Format>
                    <Include>[KMyMoney.Net]*</Include>
                    <Exclude>[*.Tests]*</Exclude>
                </Configuration>
            </DataCollector>
        </DataCollectors>
    </DataCollectionRunSettings>
</RunSettings>
```

#### Generate Coverage Report
```bash
# Generate HTML report
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

### Code Pattern Analysis

#### Find TODO/FIXME Comments
```bash
grep -rn "TODO\|FIXME\|HACK\|XXX" . --include="*.cs" --color
```

#### Find Hardcoded Strings
```bash
# Find string literals (potential localization candidates)
grep -rn '"[^"]*"' . --include="*.cs" | grep -v "//" | head -50
```

#### Find Magic Numbers
```bash
# Find numeric literals
grep -rn "[0-9][0-9]*\.[0-9]\+" . --include="*.cs" | grep -v "//" | head -20
```

### AGENTS.md Compliance Checklist

#### Critical Rules
- [ ] No US-based LLM usage (enforced by project policy)
- [ ] AGPL-3.0 license respected

#### Development Rules
- [ ] One class per file
- [ ] TDD approach used
- [ ] Minimal changes made
- [ ] No new dependencies without permission
- [ ] Existing dependencies updated to latest stable

#### Testing Rules
- [ ] Shouldly used for assertions
- [ ] NSubstitute used for mocking
- [ ] Arrange-Act-Assert pattern used

#### Code Style
- [ ] Matches existing patterns
- [ ] Matches existing naming conventions
- [ ] Matches existing error handling

### Automated Analysis Script

Create `analyze.sh`:
```bash
#!/bin/bash

echo "=== AGENTS.md Compliance Analysis ==="
echo

# One class per file check
echo "--- One Class Per File ---"
find . -name "*.cs" -type f | while read file; do
    class_count=$(grep -c "^[[:space:]]*public\s\+class\s\+" "$file" 2>/dev/null || echo 0)
    interface_count=$(grep -c "^[[:space:]]*public\s\+interface\s\+" "$file" 2>/dev/null || echo 0)
    total=$((class_count + interface_count))
    if [ "$total" -gt 1 ]; then
        echo "VIOLATION: $file has $total top-level types"
    fi
done
echo

# TDD coverage check
echo "--- Test Coverage (Production vs Test Classes) ---"
# Count production classes
prod_classes=$(grep -rh "^[[:space:]]*public\s\+class\s\+" . --include="*.cs" | \
    grep -v "Tests\|\.Tests\." | \
    wc -l)
# Count test classes
test_classes=$(grep -rh "^[[:space:]]*public\s\+class\s\+" . --include="*.cs" | \
    grep "Tests" | \
    wc -l)
echo "Production classes: $prod_classes"
echo "Test classes: $test_classes"
echo

# Unused dependencies
echo "--- Unused Dependencies ---"
find . -name "*.csproj" -type f | while read proj; do
    unused=$(dotnet list "$proj" package --unused 2>/dev/null)
    if [ -n "$unused" ]; then
        echo "$proj:"
        echo "$unused"
    fi
done
echo

# Naming convention check
echo "--- Naming Conventions ---"
echo "Interfaces not starting with I:"
grep -rh "^[[:space:]]*public\s\+interface\s\+" . --include="*.cs" | \
    grep -vE "^[[:space:]]*public\s\+interface\s\+I[A-Z]" | \
    head -5
echo

# TODO comments
echo "--- TODO/FIXME Comments ---"
grep -rn "TODO\|FIXME" . --include="*.cs" | head -5
echo

echo "=== Analysis Complete ==="
```

### Run Full Analysis
```bash
chmod +x analyze.sh
./analyze.sh 2>&1 | tee analysis-report.txt
```

## Best Practices

1. **Run analysis before major changes**
2. **Fix violations incrementally**
3. **Document exceptions** in code comments
4. **Use automated tools** where possible
5. **Review manually** for complex cases
6. **Update analysis scripts** as rules change
