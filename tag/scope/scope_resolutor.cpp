#include<iostream>
#include<cstdio>
using namespace std;

int a = 2;              // global variable

int main()
{
    
    printf("a = %d\n", a);
    int a = 10;                         //outermost local variable
    printf("a = %d\n", a);
    printf("::a = %d\n", ::a);

    printf("Loop\n");
    for(int i=0; i<2; i++){
        int a = 20;                     //inner local variable
        printf("----a = %d\n", a);
        printf("----::a = %d\n", ::a);
    }

    /*output
        a = 2
        a = 10
        ::a = 2
        Loop
        ----a = 20
        ----::a = 2
        ----a = 20
        ----::a = 2
    */
    
    cout << ":end:" <<endl;
    return 0;
}

