#include<iostream>
#include<cstdio>
using namespace std;

int a = 2; // declaración de una variable global a

int main()
{
    
    printf("a = %d\n", a);
    int a = 10;
    printf("a = %d\n", a);
    printf("::a = %d\n", ::a);
    
    /*output
        a = 2
        a = 10
        ::a = 2
    */
    
    cout << ":end:" <<endl;
    return 0;
}

