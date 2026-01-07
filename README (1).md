# SalesDataPipeline — DSL to C Translation

This repository documents the translation of a custom **pipeline-oriented DSL** into **C code**, using an AI-assisted approach.  
The pipeline processes CSV data by loading input, filtering records, mapping new fields, and saving the result.

---

## 📌 DSL Grammar Overview

The DSL supports structured data pipelines with the following core constructs:

- `PIPELINE`
- `LOAD`
- `FILTER`
- `MAP`
- `SAVE`
- Expressions, conditions, and assignments

### Grammar (excerpt)

```ebnf
PROGRAM -> PIPELINE id { STATEMENT_LIST }

STATEMENT -> LOAD_STMT
           | SAVE_STMT
           | FILTER_STMT
           | MAP_STMT
           | IF_STMT
           | FOR_STMT
           | WHILE_STMT
           | LOG_STMT
           | ASSIGN_STMT

LOAD_STMT   -> LOAD EXPR
SAVE_STMT   -> SAVE EXPR
FILTER_STMT -> FILTER CONDITION
MAP_STMT    -> MAP ASSIGNMENT_LIST
```

---

## 🧠 Source DSL Program

```dsl
PIPELINE SalesDataPipeline {
    LOAD "sales_data.csv"

    FILTER region == "RO" AND amount > 1000

    MAP total_with_tax = amount * 1.08, region_code = "RO"

    SAVE "filtered_sales_ro.csv"
}
```

---

## ⚙️ Semantics & Assumptions

- `LOAD` reads from an existing CSV file
- `SAVE` writes to a new or existing CSV file
- CSV headers are used as column identifiers
- `FILTER` removes rows that do not satisfy the condition
- `MAP` adds or overwrites columns
- Numeric values are formatted to **2 decimal places**

---

## 🧾 Sample Input

**File:** `sales_data.csv`

```csv
id,region,amount,product
1,RO,1500,TV
2,RO,800,Phone
3,US,2000,Laptop
4,RO,2500,Monitor
5,DE,1200,Printer
```

---

## 📤 Expected Output

**File:** `filtered_sales_ro.csv`

```csv
id,region,amount,product,total_with_tax,region_code
1,RO,1500,TV,1620.00,RO
4,RO,2500,Monitor,2700.00,RO
```

---

## 🧪 Validation with GCC

### Compile

```bash
gcc -std=c11 -Wall -Wextra -O2 pipeline.c -o pipeline
```

### Run

```bash
./pipeline
```

---

## 🤖 AI Model Used

- **ChatGPT (OpenAI)**
- **Model:** GPT-5.2
- AI-assisted DSL analysis and C code generation

---

## 📁 Repository Structure

```text
.
├── pipeline.c
├── sales_data.csv
├── filtered_sales_ro.csv
└── README.md
```

---

## 📜 License

Educational / academic use.
