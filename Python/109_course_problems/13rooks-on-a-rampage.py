def safe_squares_rooks(n: int, rooks: list):
    row_set = set()
    column_set = set()
    for rook in rooks:
        row_set.add(rook[0])
        column_set.add(rook[1])
    safe_row = n - len(row_set)
    safe_column = n - len(column_set)
    return safe_row * safe_column


rks = [(r, (r*(r-1))%100) for r in range(0, 100, 2)]
board = 100

print(safe_squares_rooks(board, rks))