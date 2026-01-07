# SalesDataPipeline — Reproducible DSL → C Translation

This repository documents **exactly the steps taken** to translate a custom DSL program into C code using an AI model, including **the precise inputs provided by the user and the outputs produced by the AI**.

---

## 1️⃣ DSL Grammar (User Input)

The following grammar was provided **verbatim** as input:

```text
NONTERMINALS: PROGRAM STATEMENT_LIST STATEMENT LOAD_STMT SAVE_STMT FILTER_STMT MAP_STMT IF_STMT ELSE_PART FOR_STMT WHILE_STMT LOG_STMT ASSIGN_STMT ASSIGNMENT_LIST ASSIGNMENT_LIST_TAIL ASSIGNMENT CONDITION CONDITION_TAIL COMP_OP EXPR EXPR_TAIL TERM TERM_TAIL FACTOR FACTOR_TAIL EXPR_LIST EXPR_LIST_TAIL ARG_LIST ARG_LIST_TAIL
TERMINALS: PIPELINE LOAD SAVE FILTER MAP IF THEN ELSE FOR IN WHILE LOG AND OR id number str { } ( ) [ ] , = + - * / == != > < >= <=
START: PROGRAM

PIPELINE SalesDataPipeline {
    LOAD "sales_data.csv"

    FILTER region == "RO" AND amount > 1000

    MAP total_with_tax = amount * 1.08, region_code = "RO"

    SAVE "filtered_sales_ro.csv"
}
- Input is taken with `LOAD` from an existing file
- Output is sent with `SAVE` to an existing or new file matching the given name
```

---

## 3️⃣ AI-Generated Output (C Code)

```code
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>

#define MAX_LINE  8192
#define MAX_COLS  256

static void rstrip(char *s) {
    size_t n = strlen(s);
    while (n > 0 && (s[n - 1] == '\n' || s[n - 1] == '\r')) s[--n] = '\0';
}

static char *ltrim(char *s) {
    while (*s && isspace((unsigned char)*s)) s++;
    return s;
}

static void trim_in_place(char *s) {
    char *p = ltrim(s);
    if (p != s) memmove(s, p, strlen(p) + 1);
    size_t n = strlen(s);
    while (n > 0 && isspace((unsigned char)s[n - 1])) s[--n] = '\0';
}

static void unquote_in_place(char *s) {
    size_t n = strlen(s);
    if (n >= 2 && ((s[0] == '"' && s[n - 1] == '"') || (s[0] == '\'' && s[n - 1] == '\''))) {
        memmove(s, s + 1, n - 2);
        s[n - 2] = '\0';
    }
}

static int split_csv_simple(char *line, char *out[], int max_out) {
    int count = 0;
    char *p = line;
    char *start = p;
    int in_quotes = 0;

    while (*p) {
        if (*p == '"') in_quotes = !in_quotes;
        else if (*p == ',' && !in_quotes) {
            *p = '\0';
            if (count < max_out) out[count++] = start;
            start = p + 1;
        }
        p++;
    }
    if (count < max_out) out[count++] = start;
    return count;
}

static int find_col(char *cols[], int n, const char *name) {
    for (int i = 0; i < n; i++) {
        char tmp[MAX_LINE];
        snprintf(tmp, sizeof(tmp), "%s", cols[i]);
        trim_in_place(tmp);
        unquote_in_place(tmp);
        if (strcmp(tmp, name) == 0) return i;
    }
    return -1;
}

static void write_csv_field(FILE *f, const char *s, int is_last) {
    int needs_quotes = 0;
    for (const char *p = s; *p; p++) {
        if (*p == ',' || *p == '"' || *p == '\n' || *p == '\r') { needs_quotes = 1; break; }
    }
    if (!needs_quotes) {
        fputs(s, f);
    } else {
        fputc('"', f);
        for (const char *p = s; *p; p++) {
            if (*p == '"') fputc('"', f);
            fputc(*p, f);
        }
        fputc('"', f);
    }
    if (!is_last) fputc(',', f);
}

int main(void) {
    const char *input_path  = "sales_data.csv";
    const char *output_path = "filtered_sales_ro.csv";

    FILE *in = fopen(input_path, "rb");
    if (!in) {
        fprintf(stderr, "Failed to open input file: %s\n", input_path);
        return 1;
    }

    FILE *out = fopen(output_path, "wb");
    if (!out) {
        fprintf(stderr, "Failed to open output file: %s\n", output_path);
        fclose(in);
        return 1;
    }

    char line[MAX_LINE];

    if (!fgets(line, sizeof(line), in)) {
        fprintf(stderr, "Empty input file.\n");
        fclose(in);
        fclose(out);
        return 1;
    }

    rstrip(line);

    char header_buf[MAX_LINE];
    snprintf(header_buf, sizeof(header_buf), "%s", line);

    char *headers[MAX_COLS];
    int hcount = split_csv_simple(header_buf, headers, MAX_COLS);
    for (int i = 0; i < hcount; i++) {
        trim_in_place(headers[i]);
        unquote_in_place(headers[i]);
    }

    int idx_region = find_col(headers, hcount, "region");
    int idx_amount = find_col(headers, hcount, "amount");
    if (idx_region < 0 || idx_amount < 0) {
        fprintf(stderr, "Required columns missing. Need: region, amount\n");
        fclose(in);
        fclose(out);
        return 1;
    }

    int idx_total_with_tax = find_col(headers, hcount, "total_with_tax");
    int idx_region_code    = find_col(headers, hcount, "region_code");

    int out_cols = hcount;
    int out_total_with_tax = idx_total_with_tax;
    int out_region_code    = idx_region_code;

    if (out_total_with_tax < 0) out_total_with_tax = out_cols++;
    if (out_region_code < 0)    out_region_code    = out_cols++;

    for (int i = 0; i < out_cols; i++) {
        const char *name = NULL;
        if (i < hcount) name = headers[i];
        else if (i == out_total_with_tax) name = "total_with_tax";
        else if (i == out_region_code)    name = "region_code";
        else name = "";

        write_csv_field(out, name, i == out_cols - 1);
    }
    fputc('\n', out);

    while (fgets(line, sizeof(line), in)) {
        rstrip(line);

        char row_buf[MAX_LINE];
        snprintf(row_buf, sizeof(row_buf), "%s", line);

        char *cells[MAX_COLS];
        int ccount = split_csv_simple(row_buf, cells, MAX_COLS);

        for (int i = 0; i < ccount; i++) {
            trim_in_place(cells[i]);
            unquote_in_place(cells[i]);
        }

        if (idx_region >= ccount || idx_amount >= ccount) continue;

        const char *region = cells[idx_region];
        char *endp = NULL;
        double amount = strtod(cells[idx_amount], &endp);
        if (endp == cells[idx_amount]) continue;

        if (!(strcmp(region, "RO") == 0 && amount > 1000.0)) continue;

        double total_with_tax = amount * 1.08;

        char total_buf[64];
        snprintf(total_buf, sizeof(total_buf), "%.2f", total_with_tax);

        for (int i = 0; i < out_cols; i++) {
            const char *val = "";

            if (i < hcount) {
                if (i < ccount) val = cells[i];
                else val = "";
            }

            if (i == out_total_with_tax) val = total_buf;
            if (i == out_region_code)    val = "RO";

            write_csv_field(out, val, i == out_cols - 1);
        }
        fputc('\n', out);
    }

    fclose(in);
    fclose(out);

    return 0;
}
```

The AI translated the DSL semantics into **pure ANSI C**, implementing:

- CSV loading
- Column-based filtering
- Derived-field mapping
- CSV output

The generated code:
- Uses **no external libraries**
- Is **deterministic**
- Is **portable**

> The full source is intentionally kept in `pipeline.c`.

---

## 4️⃣ Sample Input File (AI Output)

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

## 5️⃣ Expected Output File (AI Output)

**File:** `filtered_sales_ro.csv`

```csv
id,region,amount,product,total_with_tax,region_code
1,RO,1500,TV,1620.00,RO
4,RO,2500,Monitor,2700.00,RO
```

### Verification Logic

| Condition | Result |
|---------|--------|
| region == "RO" | Required |
| amount > 1000 | Required |
| total_with_tax | amount × 1.08 |
| region_code | Constant `"RO"` |

---

## 6️⃣ Validation With GCC

### Compilation

```bash
gcc -std=c11 -Wall -Wextra -O2 pipeline.c -o pipeline
```

### Execution

```bash
./pipeline
```

### Outcome

- Input CSV read successfully
- Filter applied correctly
- Derived columns generated
- Output CSV written successfully

✅ **Behavior reproduced end-to-end**

---

## 7️⃣ AI Model Used

- **ChatGPT (OpenAI)**
- **Model:** GPT-5.2
- **Usage:** Grammar understanding, semantic mapping, C code generation, documentation

No manual edits were required to obtain the final result.

---

## 8️⃣ Reproducibility Checklist

- [x] Grammar documented
- [x] DSL input documented
- [x] AI output documented
- [x] Sample data included
- [x] Output verified
- [x] Compiler validated

---

## 9️⃣ Repository Layout

```text
.
├── pipeline.c
├── sales_data.csv
├── filtered_sales_ro.csv
└── README.md
```

---

## 📜 License

Educational and academic use.

---
