<body id="slot">
  <div id="wrapper">

<div id="title">
<img src="images1/mytitle.gif" alt="FIRE AND ICE" />
</div>

<p>Take a Spin!</p>

<div id="pics">

<?php
$peach="<p><img src=\"images/peach.jpg\" alt=\"Peach\" /></p>";
$apple="<p><img src=\"images/apples.jpg\" alt=\"Apples\" /></p>";
$cherry="<p><img src=\"images/cherry.jpg\" alt=\"Cherry\" /></p>";
$coin="<p><img src=\"images/coin.jpg\" alt=\"Coin\" /></p>";
$slots = array("$peach", "$apple","$cherry","$coin");

$one_t=rand(0,3);
$two_t=rand(0,3);
$three_t=rand(0,3);

$one=$slots[$one_t];
$two=$slots[$two_t];
$three=$slots[$three_t];

print("<ul id=\"slots\">
  <li> $one </li>
  <li> $two </li>
  <li> $three </li>
  </ul>");
?>

</div>
<div id="control">
  <form action="index1.php" method="get" id="lever">
  <div id="betSelector">

  <p id="money"> Bank: 10 </p>

  <p id="bet"> Your bet:

<select name="machine">
   
  <option>20 </option>
  <option>50 </option>
  <option>100 </option>
  <option>200 </option>
 
</select>

<?php
if(isset($_GET["bet"])) {
$bet = $_GET["bet"];
}
else {
$bet = 20;
}
?>
</p>

</div> <!-- bet selector -->

<div id="spin">
<button class="spin" type="submit" name="spin">SPIN</button>
</div>
<p>
<input type="hidden" name="bank"   value="10"   />
<input type="hidden" name="bet"    value="20"   />
<input type="hidden" name="one"    value="50"   />
<input type="hidden" name="two"    value="100"   />
<input type="hidden" name="three"   value="200"   />
</p>

  </form>


  </div> <!-- control -->
 
<div id="messages">

       
    <!-- THE BANK -->
    <!-- bank has 10 ,
  fmt_money($bank) gives $10 -->

<?php


$one=$slots[$one_t];
$two=$slots[$two_t];
$three=$slots[$three_t];

[color=#33FF33]if ($one == $two == $three) {
print ("Congratulations, you won!");
}
else { print("Try Again");
}

if ($one == $two || $two == $three) {
print("Congratulations you won something!");
}else {
print("Sorry, better luck next time");
?>[/color]</p>      

 
</div> <!-- messages -->    
   
    <!-- MACHINE TYPE -->
    <div id="walkAway">
  <form action="index1.php" method="get" id="machines">
  <p>
<input type="hidden" name="bank"    value="10"   />
<input type="hidden" name="bet_dollars"     value="20"   />
<input type="hidden" name="one"     value="50"   />
<input type="hidden" name="two"     value="100"   />
<input type="hidden" name="three"   value="200"   />
</p>

    <div id="walk">
  <button class="walk" type="submit" name="walk" value"away">Walk Away </button>
    </div>
    </form>
    </div> <!--walkAway -->
   
 
    </div><!-- wrapper -->

    </body>
    </html>
