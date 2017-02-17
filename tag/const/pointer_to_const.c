#include <stdio.h>

void main(void)
{
    const int i = 2;
    int *p;
    p = &i;         //only produces a warning with c compiler
    *p = 3;
    printf("i = %d\n", i);
}