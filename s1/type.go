package main
import (
	"fmt"
	// "unsafe"
)
func s1_type(){
	//Bool
	// a := true
    // b := false
    // fmt.Println("a:", a, "b:", b)
    // c := a && b
    // fmt.Println("c:", c)
    // d := a || b
	// fmt.Println("d:", d)
	
	//int
	// var a int = 89
    // b := 95
    // fmt.Println("value of a is", a, "and b is", b)
    // fmt.Printf("type of a is %T, size of a is %d", a, unsafe.Sizeof(a)) //type and size of a
    // fmt.Printf("\ntype of b is %T, size of b is %d", b, unsafe.Sizeof(b)) //type and size of b

	// //Float
	// a, b := 5.67, 8.97
    // fmt.Printf("type of a %T b %T\n", a, b)
    // sum := a + b
    // diff := a - b
    // fmt.Println("sum", sum, "diff", diff)

    // no1, no2 := 56, 89
	// fmt.Println("sum", no1+no2, "diff", no1-no2)

	// //complex
	// c1 := complex(5, 7)
    // c2 := 8 + 27i
    // cadd := c1 + c2
    // fmt.Println("sum:", cadd)
    // cmul := c1 * c2
	// fmt.Println("product:", cmul)
	
	// //string
	// first := "Naveen"
    // last := "Ramanathan"
    // name := first +" "+ last
	// fmt.Println("My name is",name)
	
	//Type conver
	i := 55      //int
    j := 67.8    //float64
    sum := i + int(j) //j is converted to int
    fmt.Println(sum)
}