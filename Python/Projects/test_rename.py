import os

os.chdir("/home/vilas/Music/")

for i, f in enumerate(os.listdir()):
    # print(f)
    file_name, file_ext = os.path.splitext(f)
    file_title, rest = file_name.split("-")
    newname = f'{str(i+1).zfill(2)}-{file_title}{file_ext}'

    os.rename(f, newname)

