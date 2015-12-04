<?
function rearrange_title($title) {
    if (preg_match('/, The$/', $title)) { $title = "The " . substr($title,0,-5); }
    if (preg_match('/, A$/', $title)) { $title = "A " . substr($title,0,-3); }
    if (preg_match('/, An$/', $title)) { $title = "An " . substr($title,0,-4); }
    return ($title);
}
?>
