class Bcolors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKCYAN = '\033[96m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'


def col_to_letter(col):
    r = ''
    while col > 0:
        v = (col - 1) % 26
        r = chr(v + 65) + r
        col = (col - v - 1) // 26
    return r


def find_auth(value, ws):
    cell = ws.find(value)
    if cell is not None:
        row_values = ws.row_values(cell.row)
        port, user_id, password = row_values[2:5]
        return f'''Found {Bcolors.OKGREEN}{cell.value}{Bcolors.ENDC} at {Bcolors.OKGREEN}{col_to_letter(cell.col)}-{cell.row}{Bcolors.ENDC}
{Bcolors.OKCYAN}{port}:{user_id}:{password}{Bcolors.ENDC}\n'''


def find_id(value, ws):
    cell = ws.find(value)
    if cell is not None:
        row_values = ws.row_values(cell.row)
        instance_id = row_values[3]
        return f"InstanceID: {instance_id} - {cell.value}"


def result_print(items):
    for item in items:
        if item is not None:
            print(item)
