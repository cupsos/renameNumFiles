# renameNumFiles
rename filenames that include number from zero

# usage
`renameNumFiles.exe [path]`

# example
```
$ls
File20.png File25.png file1.txt file2.txt file50.txt nonNumFile numDir32
$renameNumFiles.exe .
rename : File20.png => File0.png
rename : File25.png => File1.png
rename : file1.txt => file0.txt
rename : file2.txt => file1.txt
rename : file50.txt => file2.txt
$ls
File0.png File1.png file0.txt file1.txt file2.txt nonNumFile numDir32
```

# warning
```
$ls
double1_2.png double2_1.png texture512.png 
$renameNumFiles.exe .
rename : double1_2.png => double1_0.png
rename : double2_1.png => double2_0.png
rename : texture512.png => texture0.png
$ls
double1_0.png double2_0.png texture0.png
```
