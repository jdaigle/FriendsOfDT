<?
include_once "include/get_fullname.php";

function EC_by_year($year) {
    $min_year_query = mysql_query ("select min(year),max(year) from EC");
    while ($min_year_result = mysql_fetch_array($min_year_query)) {
	$min_year = $min_year_result[0];
	$max_year = $min_year_result[1];
    }
    if (!isset($year)) {$year=$max_year;}
    if ($year > $max_year) {$year=$max_year;}
    if ($year < $min_year) {$year=$min_year;}
    $order_select = mysql_query("SELECT distinct year FROM EC order by year");
    while ($order_result = mysql_fetch_array($order_select)) {
        if (isset($next)) {
            if ($next == 'next') {
                $next = $order_result[0];
            }
        }
        if ($order_result[0] == $year && !isset($next)) {
            $next = 'next';
        } elseif (!isset($next)) {
            $last = $order_result[0];
        }
    }
    if ($last > 0) {
        $prevlink = "<a href=\"$_SERVER[PHP_SELF]?action=EC_by_year&amp;year=$last\"><--- Previous Year</a>";
    } else {
        $prevlink = "<--- Previous Year";
    }
    if ($next == 'next') {
        $nextlink = "Next Year --->";
    } else {
        $nextlink = "<a href=\"$_SERVER[PHP_SELF]?action=EC_by_year&amp;year=$next\">Next Year ---></a>\n";
    }

    echo "<table width=80% align=center><tr><td width=25%>$prevlink</td><td><center><h1>DT Executive Council for $year</h1></center></td><td width=25% align=right>$nextlink</td></tr></table><center>";
    $EC_select = mysql_query("select e.peepID,l.title from EC e INNER JOIN EC_list l ON e.ECID=l.ID where e.year=$year order by e.ECID,e.peepID");
    while ($result = mysql_fetch_array($EC_select)) {
        $name=get_fullname($result[0]);
	$title = $result[1];
	echo "$title - <a href=\"?action=peep_detail&amp;peep_id=$result[0]\">$name</a><br>\n";
    }
    echo "</center>";
}
?>
